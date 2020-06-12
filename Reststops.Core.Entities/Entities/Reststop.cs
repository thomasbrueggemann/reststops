using System.Collections.Generic;
using Reststops.Domain.Enums;

namespace Reststops.Domain.Entities
{
    public class Reststop
    {
        public ulong ID { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public ReststopType Type { get; set; }
        public Dictionary<string, string> Tags { get; set; }
    }
}
