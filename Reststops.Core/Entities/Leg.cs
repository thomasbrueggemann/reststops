using System.Collections.Generic;

namespace Reststops.Core.Entities
{
    public record Leg
    {
        public string Summary { get; init; }
        public double Weight { get; init; }
        public double Duration { get; init; }
        public List<object> Steps { get; init; }
        public double Distance { get; init; }
    }
}
