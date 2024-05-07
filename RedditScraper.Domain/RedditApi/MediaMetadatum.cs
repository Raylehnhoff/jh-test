using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

public partial class MediaMetadatum
{
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("e")]
    public string E { get; set; }

    [JsonProperty("m")]
    public string M { get; set; }

    [JsonProperty("p")]
    public S[] P { get; set; }

    [JsonProperty("s")]
    public S S { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }
}