using Newtonsoft.Json;
using RedditScraper.Domain.RedditApi;

namespace RedditScraper.Domain;

public partial class SubredditListing
{
    public static SubredditListing FromJson(string json) => JsonConvert.DeserializeObject<SubredditListing>(json, Converter.Settings);
}

public partial class SubredditListing
{
    [JsonProperty("kind")]
    public string Kind { get; set; }

    [JsonProperty("data")]
    public RedditPost_Data Data { get; set; }
}