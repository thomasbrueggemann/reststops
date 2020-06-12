using System.Collections.Generic;

namespace Reststops.Domain.Entities
{
    public class Waypoint
    {
        public double Distance { get; set; }
        public string Name { get; set; }
        public List<double> Location { get; set; }
    }
}
