using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

public partial class S
{
    [JsonProperty("y")]
    public long Y { get; set; }

    [JsonProperty("x")]
    public long X { get; set; }

    [JsonProperty("u")]
    public Uri U { get; set; }
}