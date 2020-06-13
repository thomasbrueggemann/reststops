namespace Reststops.Core.Entities
{
    public class ForwardGeocoding
    {
        public string Type { get; set; }
        public string[] Query { get; set; }
        public ForwardGeocodingFeature[] Features { get; set; }
        public string Attribution { get; set; }
    }
}
