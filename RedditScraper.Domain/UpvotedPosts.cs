using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditScraper.Domain
{
    public record UpvotedPosts
    {
        public string Title { get; set; }
        public long Upvotes { get; set; }
        public string UniqueId { get; set; }
        public string Subreddit { get; set; }
    }
}
