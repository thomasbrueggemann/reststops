using System.Collections.Generic;
using Reststops.Core.Enums;

namespace Reststops.Core.Entities
{
    public record Reststop
    {
        public ulong ID { get; init; }
        public string Name { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public ReststopType Type { get; init; }
        public Dictionary<string, string> Tags { get; init; }
    }
}
