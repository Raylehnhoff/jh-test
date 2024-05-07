using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditScraper.Poller
{
    public class ScraperOptions
    {
        [ConfigurationKeyName("UserAgent")]
        public string UserAgent { get; set; }

        [ConfigurationKeyName("Subreddits")]
        public List<string> Subreddits { get; set; } = new List<string>();

        [ConfigurationKeyName("Sort")] public string Sort { get; set; }
        [ConfigurationKeyName("TopNUsers")] public int TopNUsers { get; set; }
        [ConfigurationKeyName("TopNPosts")] public int TopNPosts { get; set; }
    }
}