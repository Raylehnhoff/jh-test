using Newtonsoft.Json;

namespace RedditScraper.Domain.RedditApi;

internal class PostHintConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(PostHint) || t == typeof(PostHint?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        switch (value)
        {
            case "image":
                return PostHint.Image;
            case "link":
                return PostHint.Link;
            case "rich:video":
                return PostHint.RichVideo;
            case "self":
                return PostHint.Self;
        }
        throw new Exception("Cannot unmarshal type PostHint");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (PostHint)untypedValue;
        switch (value)
        {
            case PostHint.Image:
                serializer.Serialize(writer, "image");
                return;
            case PostHint.Link:
                serializer.Serialize(writer, "link");
                return;
            case PostHint.RichVideo:
                serializer.Serialize(writer, "rich:video");
                return;
            case PostHint.Self:
                serializer.Serialize(writer, "self");
                return;
        }
        throw new Exception("Cannot marshal type PostHint");
    }

    public static readonly PostHintConverter Singleton = new PostHintConverter();
}