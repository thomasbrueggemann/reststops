using Newtonsoft.Json;

namespace Reststops.Domain.Entities
{
    public class ForwardGeocodingFeature
    {
        public string Id { get; set; }
        public string Type { get; set; }

        [JsonProperty("place_type")]
        public string[] PlaceType { get; set; }
        public long Relevance { get; set; }
        public string Text { get; set; }

        [JsonProperty("place_name")]
        public string PlaceName { get; set; }
        public double[] Bbox { get; set; }
        public double[] Center { get; set; }

        [JsonProperty("matching_text")]
        public string MatchingText { get; set; }

        [JsonProperty("matching_place_name")]
        public string MatchingPlaceName { get; set; }
    }
}
