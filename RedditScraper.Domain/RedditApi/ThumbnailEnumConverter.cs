using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

internal class ThumbnailEnumConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(ThumbnailEnum) || t == typeof(ThumbnailEnum?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        switch (value)
        {
            case "default":
                return ThumbnailEnum.Default;
            case "self":
                return ThumbnailEnum.Self;
        }
        throw new Exception("Cannot unmarshal type ThumbnailEnum");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (ThumbnailEnum)untypedValue;
        switch (value)
        {
            case ThumbnailEnum.Default:
                serializer.Serialize(writer, "default");
                return;
            case ThumbnailEnum.Self:
                serializer.Serialize(writer, "self");
                return;
        }
        throw new Exception("Cannot marshal type ThumbnailEnum");
    }

    public static readonly ThumbnailEnumConverter Singleton = new ThumbnailEnumConverter();
}