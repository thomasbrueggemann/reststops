using System.Collections.Generic;

namespace Reststops.Core.Entities
{
    public record Route
    {
        public Geometry Geometry { get; init; }
        public List<Leg> Legs { get; init; }
        public string WeightName { get; init; }
        public double Weight { get; init; }
        public double Duration { get; init; }
        public double Distance { get; init; }
    }
}
