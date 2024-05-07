using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

public partial class Source
{
    [JsonProperty("url")]
    public Uri Url { get; set; }

    [JsonProperty("width")]
    public long Width { get; set; }

    [JsonProperty("height")]
    public long Height { get; set; }
}