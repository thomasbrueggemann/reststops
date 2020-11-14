using System.Collections.Generic;

namespace Reststops.Core.Entities
{
    public record Waypoint
    {
        public double Distance { get; init; }
        public string Name { get; init; }
        public List<double> Location { get; init; }
    }
}
