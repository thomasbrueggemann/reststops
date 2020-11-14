using System.Collections.Generic;

namespace Reststops.Presentation.API.Models
{
    public record ReststopModel
    {
        public ulong ID { get; init; }

        public string Name { get; init; }

        public double Latitude { get; init; }

        public double Longitude { get; init; }

        public string Type { get; init; }

        public Dictionary<string, string> Tags { get; init; }

        public int DistanceInMeters { get; init; }

        public int DetourDurationInSeconds { get; init; }
    }
}
