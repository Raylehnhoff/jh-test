using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditScraper.Domain
{
    public record UserPosts
    {
        public string UserId { get; set; }
        public HashSet<string> PostIds { get; set; } = new HashSet<string>();
        public int PostCount => PostIds.Count;
    }
}
