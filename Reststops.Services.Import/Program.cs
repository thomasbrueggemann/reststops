using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reststops.Domain.Entities;
using Reststops.Domain.Enums;

namespace Reststops.Services.Import
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            string jsonString = await File.ReadAllTextAsync("reststops.json");

            dynamic data = JsonConvert.DeserializeObject(jsonString);

            foreach(var element in data.elements)
            {
                try
                {
                    if (element.lat == null || element.lon == null) continue;

                    var tags = ((JObject)element.tags).ToObject<Dictionary<string, string>>();

                    var reststop = new Reststop
                    (
                        ID: (ulong)element.id,
                        Name: tags.GetValueOrDefault("name", null),
                        Latitude: (decimal)element.lat,
                        Longitude: (decimal)element.lon,
                        Type: tags.GetValueOrDefault("highway", "rest_area") == "services"
                                ? ReststopType.ServiceArea
                                : ReststopType.RestArea,
                        Tags: tags
                    );

                    Console.WriteLine(reststop.ID);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return 0;
        }
    }
}
