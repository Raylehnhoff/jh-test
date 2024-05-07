using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

public static class Serialize
{
    public static string ToJson(this SubredditListing self) => JsonConvert.SerializeObject(self, Converter.Settings);
}