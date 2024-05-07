using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

public partial class RedditPost_Data
{
    [JsonProperty("after")]
    public string After { get; set; }

    [JsonProperty("dist")]
    public long Dist { get; set; }

    [JsonProperty("modhash")]
    public string Modhash { get; set; }

    [JsonProperty("geo_filter")]
    public string GeoFilter { get; set; }

    [JsonProperty("children")]
    public IEnumerable<Child> Children { get; set; }

    [JsonProperty("before")]
    public object Before { get; set; }
}