using System.Collections.Generic;
using Reststops.Domain.Enums;

namespace Reststops.Domain.Entities
{
    public class Reststop
    {
        public ulong ID { get; private set; }
        public string Name { get; private set; }
        public decimal Latitude { get; private set; }
        public decimal Longitude { get; private set; }
        public ReststopType Type { get; private set; }
        public Dictionary<string, string> Tags { get; private set; }

        public Reststop(
            ulong ID,
            string Name,
            decimal Latitude,
            decimal Longitude,
            ReststopType Type,
            Dictionary<string, string> Tags
        )
        {
            this.ID = ID;
            this.Name = Name;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Type = Type;
            this.Tags = Tags;
        }
    }
}
