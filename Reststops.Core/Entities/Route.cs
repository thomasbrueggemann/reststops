using System.Collections.Generic;

namespace Reststops.Core.Entities
{
    public class Route
    {
        public Geometry Geometry { get; set; }
        public List<Leg> Legs { get; set; }
        public string WeightName { get; set; }
        public double Weight { get; set; }
        public double Duration { get; set; }
        public double Distance { get; set; }
    }
}
