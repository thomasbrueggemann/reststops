using System.Collections.Generic;

namespace Reststops.Core.Entities
{
    public record DirectionsRoute
    {
        public List<Route> Routes { get; init; }
        public List<Waypoint> Waypoints { get; init; }
        public string Code { get; init; }
        public string Uuid { get; init; }
    }
}
