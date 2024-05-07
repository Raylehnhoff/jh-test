using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

public partial class Media
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("oembed")]
    public Oembed Oembed { get; set; }
}