using System.Collections.Generic;

namespace Reststops.Core.Entities
{
    public class Leg
    {
        public string Summary { get; set; }
        public double Weight { get; set; }
        public double Duration { get; set; }
        public List<object> Steps { get; set; }
        public double Distance { get; set; }
    }
}
