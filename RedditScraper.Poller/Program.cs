using System.Net.Http.Headers;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using RedditScraper.Domain;
using RedditScraper.Poller.Services;
using RedditScraper.Poller.Workers;

namespace RedditScraper.Poller
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            //unbounded are dangerous because they can cause OOM exception, but given we're polling on a 1.5s timer I think we're okay.
            var redditPostChannel = Channel.CreateUnbounded<SubredditListing>();
            builder.Services.AddSingleton(redditPostChannel);

            //These two channels will report statistics updates with the top posts.
            var userPostsChannel = Channel.CreateBounded<List<UserPosts>>(1);
            var upvotedPostsChannel = Channel.CreateBounded<List<UpvotedPosts>>(1);

            builder.Services.AddSingleton(userPostsChannel);
            builder.Services.AddSingleton(upvotedPostsChannel);

            builder.Services.AddHostedService<PollWorker>();
            builder.Services.AddHostedService<IngestionWorker>();
            builder.Services.AddHostedService<UserReportingWorker>();
            builder.Services.AddHostedService<PostReportingWorker>();
            builder.Services.AddSingleton<DataStorage>();
            builder.Services.AddHttpClient("RedditHttpClient", (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ScraperOptions>>();
                client.DefaultRequestHeaders.UserAgent.ParseAdd(options.Value.UserAgent);
            });
            builder.Services.Configure<ScraperOptions>(builder.Configuration.GetSection("Scraper"));
            var host = builder.Build();
            host.Run();
        }
    }
}