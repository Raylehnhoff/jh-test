using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

public partial class Image
{
    [JsonProperty("source")]
    public Source Source { get; set; }

    [JsonProperty("resolutions")]
    public Source[] Resolutions { get; set; }

    [JsonProperty("variants")]
    public Gildings Variants { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }
}