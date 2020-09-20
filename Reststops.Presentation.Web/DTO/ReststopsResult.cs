using System;
using System.Collections.Generic;

namespace Reststops.Presentation.Web.DTO
{
    public class ReststopResult
    {
        public ulong ID { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public int DistanceInMeters { get; set; }
        public int DetourDurationInSeconds { get; set; }

        public bool HasTag(string key, string value)
            => Tags.ContainsKey(key) && Tags[key] == value;

        public bool HasTag(string key)
            => Tags.ContainsKey(key);

        public string KilometersAway() => $"{Math.Round((double) DistanceInMeters / 1000.0, 1)} km";

        public string DetourInMinutes() => $"+{(int)(DetourDurationInSeconds / 60)} min";

        public int ListIndex { get; set; }
    }

    public class ReststopsResult
    {
        public IEnumerable<ReststopResult> Reststops { get; set; } = new List<ReststopResult>();
        public string Route { get; set; }
        public string Corridor { get; set; }
    }
}
