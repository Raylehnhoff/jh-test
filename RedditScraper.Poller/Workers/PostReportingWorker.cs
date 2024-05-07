using RedditScraper.Domain;
using System.Threading.Channels;

namespace RedditScraper.Poller.Workers
{
    public class PostReportingWorker : BackgroundService
    {
        private readonly Channel<List<UpvotedPosts>> _upvotedPostsChannel;

        public PostReportingWorker(Channel<List<UpvotedPosts>> upvotedPostsChannel)
        {
            _upvotedPostsChannel = upvotedPostsChannel;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await foreach (var upvotedPosts in _upvotedPostsChannel.Reader.ReadAllAsync(stoppingToken))
                {
                    Console.WriteLine($"The current top posts in the \"{upvotedPosts.First().Subreddit}\" subreddit are");
                    foreach (var post in upvotedPosts)
                    {
                        Console.WriteLine("{0,5} | {1,50}", post.Upvotes, post.Title);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}