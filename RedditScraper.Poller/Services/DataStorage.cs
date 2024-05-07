using RedditScraper.Domain;
using RedditScraper.Domain.RedditApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace RedditScraper.Poller.Services
{
    public class DataStorage
    {
        private readonly IOptions<ScraperOptions> _options;
        private readonly Channel<List<UserPosts>> _topUserPostsChannel;
        private readonly Channel<List<UpvotedPosts>> _upvotedPostsChannel;
        private readonly ConcurrentDictionary<string, UpvotedPosts> _upvotedPosts = new ConcurrentDictionary<string, UpvotedPosts>();
        private readonly ConcurrentDictionary<string, UserPosts> _userPosts = new ConcurrentDictionary<string, UserPosts>();

        public DataStorage(IOptions<ScraperOptions> options, Channel<List<UserPosts>> topUserPostsChannel, Channel<List<UpvotedPosts>> upvotedPostsChannel)
        {
            _options = options;
            _topUserPostsChannel = topUserPostsChannel;
            _upvotedPostsChannel = upvotedPostsChannel;
        }

        public async Task UpdateStorage(Child item)
        {
            _upvotedPosts.AddOrUpdate(item.Data.Id, (postId) => new UpvotedPosts()
            {
                Title = item.Data.Title,
                UniqueId = postId,
                Upvotes = item.Data.Ups,
                Subreddit = item.Data.Subreddit
            }, (s, posts) =>
            {
                posts.Upvotes = item.Data.Ups;
                return posts;
            });

            _userPosts.AddOrUpdate($"{item.Data.Author}", (authorId) => new UserPosts()
            {
                UserId = authorId,
                PostIds = new HashSet<string>()
                {
                    item.Data.Id
                }
            }, (authorId, post) =>
            {
                post.PostIds.Add(item.Data.Id);
                return post;
            });

            foreach (var subredditPostSet in _upvotedPosts.Values.GroupBy(k => k.Subreddit, v => v))
            {
                await Task.Run(() => _upvotedPostsChannel.Writer.WriteAsync(subredditPostSet.OrderByDescending(p => p.Upvotes).Take(_options.Value.TopNPosts)
                    .ToList()));
            }

            await _topUserPostsChannel.Writer.WriteAsync(_userPosts.Values.OrderByDescending(p => p.PostCount).Take(_options.Value.TopNUsers)
                .ToList());
        }
    }
}