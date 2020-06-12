using System.Collections.Generic;

namespace Reststops.Domain.Entities
{
    public class DirectionsRoute
    {
        public List<Route> Routes { get; set; }
        public List<Waypoint> Waypoints { get; set; }
        public string Code { get; set; }
        public string Uuid { get; set; }
    }
}
