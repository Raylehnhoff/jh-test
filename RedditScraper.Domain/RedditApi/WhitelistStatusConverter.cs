using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

internal class WhitelistStatusConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(WhitelistStatus) || t == typeof(WhitelistStatus?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        if (value == "all_ads")
        {
            return WhitelistStatus.AllAds;
        }
        throw new Exception("Cannot unmarshal type WhitelistStatus");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (WhitelistStatus)untypedValue;
        if (value == WhitelistStatus.AllAds)
        {
            serializer.Serialize(writer, "all_ads");
            return;
        }
        throw new Exception("Cannot marshal type WhitelistStatus");
    }

    public static readonly WhitelistStatusConverter Singleton = new WhitelistStatusConverter();
}