using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

internal class SubredditTypeConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(SubredditType) || t == typeof(SubredditType?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        if (value == "public")
        {
            return SubredditType.Public;
        }
        throw new Exception("Cannot unmarshal type SubredditType");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (SubredditType)untypedValue;
        if (value == SubredditType.Public)
        {
            serializer.Serialize(writer, "public");
            return;
        }
        throw new Exception("Cannot marshal type SubredditType");
    }

    public static readonly SubredditTypeConverter Singleton = new SubredditTypeConverter();
}