using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

internal class KindConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(Kind) || t == typeof(Kind?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        return value switch
        {
            "t1" => Kind.Comment,
            "t2" => Kind.Account,
            "t3" => Kind.Link,
            "t4" => Kind.Message,
            "t5" => Kind.Subreddit,
            "t6" => Kind.Award,
            _ => throw new Exception("Cannot unmarshal type Kind: " + value)
        };
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (Kind)untypedValue;
        switch (value)
        {
            case Kind.Comment:
                serializer.Serialize(writer, "t1");
                return;
            case Kind.Account:
                serializer.Serialize(writer, "t2");
                return;
            case Kind.Link:
                serializer.Serialize(writer, "t3");
                return;
            case Kind.Message:
                serializer.Serialize(writer, "t4");
                return;
            case Kind.Subreddit:
                serializer.Serialize(writer, "t3");
                return;
            case Kind.Award:
                serializer.Serialize(writer, "t6");
                return;
            default:
                throw new Exception("Cannot marshal type Kind: " + value);
        }
    }

    public static readonly KindConverter Singleton = new KindConverter();
}