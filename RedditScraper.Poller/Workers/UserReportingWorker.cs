using Microsoft.Extensions.Hosting;
using RedditScraper.Domain;
using System.Threading.Channels;

namespace RedditScraper.Poller.Workers
{
    public class UserReportingWorker : BackgroundService
    {
        private readonly Channel<List<UserPosts>> _topUserPostsChannel;
        private readonly Channel<List<UpvotedPosts>> _upvotedPostsChannel;

        public UserReportingWorker(Channel<List<UserPosts>> topUserPostsChannel, Channel<List<UpvotedPosts>> upvotedPostsChannel)
        {
            _topUserPostsChannel = topUserPostsChannel;
            _upvotedPostsChannel = upvotedPostsChannel;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await foreach (var userPosts in _topUserPostsChannel.Reader.ReadAllAsync(stoppingToken))
                {
                    Console.WriteLine("The current top users are");
                    foreach (var user in userPosts)
                    {
                        Console.WriteLine("{0,4} | {1,20}", user.PostCount, user.UserId);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}