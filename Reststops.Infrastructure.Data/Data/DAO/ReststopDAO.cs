﻿using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Spatial;
using Newtonsoft.Json;

namespace Reststops.Infrastructure.Data.DAO
{
    public class ReststopDAO
    {
        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("location")]
        public Point Location { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("tags")]
        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
    }
}
