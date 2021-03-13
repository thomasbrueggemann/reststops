using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos.Spatial;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using Newtonsoft.Json;

namespace Reststops.Infrastructure.Data.DAO
{
    public class ReststopDAO
    {
        [JsonProperty("_id")]
        [BsonId]
        public ObjectId _Id { get; set; } = new ObjectId();

        [JsonProperty("id")]
        [Key]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("location")]
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; } = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
            new GeoJson2DGeographicCoordinates(0, 0));

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("tags")]
        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
    }
}
