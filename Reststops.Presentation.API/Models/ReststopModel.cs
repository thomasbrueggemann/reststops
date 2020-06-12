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
        public double DistanceInMeters { get; set; }
        public double DurationInSeconds { get; set; }

        public ReststopModel WithDistanceAndDuration(double distance, double duration)
        {
            DistanceInMeters = distance;
            DurationInSeconds = duration;

            return this;
        }
    }
}
