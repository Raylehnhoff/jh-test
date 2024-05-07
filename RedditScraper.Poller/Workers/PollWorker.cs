using System.Linq;
using System.Net;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using Polly;
using RedditScraper.Domain;

namespace RedditScraper.Poller.Workers
{
    /// <summary>
    /// The purpose of this worker is to parse channel data and to expose it to a data storage system (in-memory here)
    /// </summary>
    public class PollWorker : BackgroundService
    {
        private readonly IOptions<ScraperOptions> _options;
        private readonly IHttpClientFactory _clientFactory;
        private readonly Channel<SubredditListing> _postChannel;
        private readonly ILogger<PollWorker> _logger;
        private static readonly Random randomDelay = new Random();
        private const int maxRetries = 3;

        public PollWorker(IOptions<ScraperOptions> options, IHttpClientFactory clientFactory, Channel<SubredditListing> postChannel, ILogger<PollWorker> logger)
        {
            _options = options;
            _clientFactory = clientFactory;
            _postChannel = postChannel;
            _logger = logger;
        }
        private static TimeSpan RetryAttempt(int retryCount)
        {
            return TimeSpan.FromMilliseconds(randomDelay.Next(1500, 3000));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var httpClient = _clientFactory.CreateClient("RedditHttpClient");
            while (!stoppingToken.IsCancellationRequested)
            {
                
                var subredditsToMonitor = string.Join('+', _options.Value.Subreddits);
                await Policy.HandleResult<HttpStatusCode>(r => r == HttpStatusCode.TooManyRequests)
                    .Or<HttpRequestException>()
                    .WaitAndRetryAsync(maxRetries, RetryAttempt, (result, duration, retryCount, context) =>
                    {
                        if (retryCount == maxRetries)
                        {
                            var exceptionMessage =
                                "Max retries exceeded for PollWorker.cs polling process; application will die.";
                            _logger.LogCritical(exceptionMessage);
                            throw new InvalidOperationException(exceptionMessage);
                        }
                    })
                    .ExecuteAsync(async () =>
                    {
                        // a little known fact is that you don't need to do the OAuth stuff with reddit; they expose ".json" URLs that are part of their cache strategy
                        // as long as you're not doing anything too crazy, you can avoid the entire OAuth handshake headache to get tokens, or storing tokens at all if you
                        // do not need a user context, which this interview problem does not.
                        using var request = await httpClient.GetAsync(
                            $"https://reddit.com/r/{subredditsToMonitor}/{_options.Value.Sort}.json", stoppingToken);
                        //throws if non-2xx
                        request.EnsureSuccessStatusCode();
                        var res = await request.Content.ReadAsStringAsync(stoppingToken);
                        var listing = SubredditListing.FromJson(res);
                        await _postChannel.Writer.WriteAsync(listing, stoppingToken);
                        return request.StatusCode;
                    });
                await Task.Delay(1500, stoppingToken);
            }
        }
    }
}
