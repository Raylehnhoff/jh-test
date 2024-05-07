using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

public partial class Preview
{
    [JsonProperty("images")]
    public Image[] Images { get; set; }

    [JsonProperty("enabled")]
    public bool Enabled { get; set; }
}