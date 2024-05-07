using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

internal class FlairTypeConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(FlairType) || t == typeof(FlairType?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        if (value == "text")
        {
            return FlairType.Text;
        }
        throw new Exception("Cannot unmarshal type FlairType");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (FlairType)untypedValue;
        if (value == FlairType.Text)
        {
            serializer.Serialize(writer, "text");
            return;
        }
        throw new Exception("Cannot marshal type FlairType");
    }

    public static readonly FlairTypeConverter Singleton = new FlairTypeConverter();
}