using System.Collections.Generic;

namespace Reststops.Presentation.Web.DTO
{
    public class MapboxPlacesResultFeature
    {
        public string id { get; set; }
        public double[] center { get; set; }
        public string text { get; set; }
        public string place_name { get; set; }
    }

    public class MapboxPlacesResult
    {
        public string type { get; set; }
        public string[] query { get; set; }
        public List<MapboxPlacesResultFeature> features { get; set; }
    }
}
