using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

public partial class GalleryData
{
    [JsonProperty("items")]
    public Item[] Items { get; set; }
}