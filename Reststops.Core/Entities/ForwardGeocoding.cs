namespace Reststops.Core.Entities
{
    public record ForwardGeocoding
    {
        public string Type { get; init; }
        public string[] Query { get; init; }
        public ForwardGeocodingFeature[] Features { get; init; }
        public string Attribution { get; init; }
    }
}
