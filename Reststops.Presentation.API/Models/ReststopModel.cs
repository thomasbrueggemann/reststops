using System.Collections.Generic;

namespace Reststops.Presentation.API.Models
{
    public class ReststopModel
    {
        public ulong ID { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> Tags { get; set; }

        // additional information for model
        public int DistanceInMeters { get; set; }
        public int DetourDurationInSeconds { get; set; }

        public ReststopModel WithDistanceAndDetourDuration(
            int distance,
            int detourDuration
        )
        {
            DistanceInMeters = distance;
            DetourDurationInSeconds = detourDuration;

            return this;
        }
    }
}
