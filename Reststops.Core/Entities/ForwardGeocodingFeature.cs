using Newtonsoft.Json;

namespace Reststops.Core.Entities
{
    public record ForwardGeocodingFeature
    {
        public string Id { get; init; }
        public string Type { get; init; }

        [JsonProperty("place_type")]
        public string[] PlaceType { get; init; }
        public long Relevance { get; init; }
        public string Text { get; init; }

        [JsonProperty("place_name")]
        public string PlaceName { get; init; }
        public double[] Bbox { get; init; }
        public double[] Center { get; init; }

        [JsonProperty("matching_text")]
        public string MatchingText { get; init; }

        [JsonProperty("matching_place_name")]
        public string MatchingPlaceName { get; init; }
    }
}
