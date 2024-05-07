using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

public partial class Item
{
    [JsonProperty("media_id")]
    public string MediaId { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }
}