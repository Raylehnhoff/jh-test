using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

public partial class Child
{
    [JsonProperty("kind")]
    public Kind Kind { get; set; }

    [JsonProperty("data")]
    public ChildData Data { get; set; }
}