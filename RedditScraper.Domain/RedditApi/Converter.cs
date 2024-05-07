using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RedditScraper.Domain.RedditApi;

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
        {
            FlairTypeConverter.Singleton,
            LinkFlairTextColorConverter.Singleton,
            WhitelistStatusConverter.Singleton,
            PostHintConverter.Singleton,
            SubredditTypeConverter.Singleton,
            ThumbnailEnumConverter.Singleton,
            KindConverter.Singleton,
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
    };
}