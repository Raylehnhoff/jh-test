using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using RedditScraper.Domain;
using RedditScraper.Domain.RedditApi;
using RedditScraper.Poller.Services;

namespace RedditScraper.Poller.Workers
{
    /// <summary>
    /// The responsibility of this worker is to make an web request to Reddit and place the response into a channel for downstream consumption
    /// </summary>
    public class IngestionWorker : BackgroundService
    {
        private readonly DataStorage _storage;
        private readonly Channel<SubredditListing> _postChannel;

        public IngestionWorker(DataStorage storage, Channel<SubredditListing> postChannel)
        {
            _storage = storage;
            _postChannel = postChannel;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await foreach (var listing in _postChannel.Reader.ReadAllAsync(stoppingToken))
                {
                    if (listing.Kind == "Listing")
                    {
                        foreach (var post in listing.Data.Children)
                            await _storage.UpdateStorage(post);
                    }
                }
            }
        }
    }
}