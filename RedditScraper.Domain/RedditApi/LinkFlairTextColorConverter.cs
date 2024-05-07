using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

internal class LinkFlairTextColorConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(LinkFlairTextColor) || t == typeof(LinkFlairTextColor?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        if (value == "dark")
        {
            return LinkFlairTextColor.Dark;
        }
        throw new Exception("Cannot unmarshal type LinkFlairTextColor");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (LinkFlairTextColor)untypedValue;
        if (value == LinkFlairTextColor.Dark)
        {
            serializer.Serialize(writer, "dark");
            return;
        }
        throw new Exception("Cannot marshal type LinkFlairTextColor");
    }

    public static readonly LinkFlairTextColorConverter Singleton = new LinkFlairTextColorConverter();
}