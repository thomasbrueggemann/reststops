using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reststops.Core.Interfaces.Repositories;
using Reststops.Core.Entities;
using Reststops.Core.Enums;
using Reststops.Infrastructure.Data;
using Reststops.Infrastructure.Data.DAO;

namespace Reststops.Services.Import
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            #region Setup DI

            //setup our DI
            var services = new ServiceCollection()
                .AddAutoMapper(
                    typeof(EntityDAOMappingProfile)
                )
                .AddInfrastructure()
                .BuildServiceProvider();

            #endregion

            string jsonString = await File.ReadAllTextAsync("reststops.json");
            dynamic data = JsonConvert.DeserializeObject(jsonString);
            var reststopRepository = services.GetService<IReststopRepository>();

            int insertCount = 0;

            Func<dynamic, double?> GetLatitude = (dynamic element) =>
            {
                if (element.center != null && element.center.lat != null)
                    return (double) element.center.lat;
               
                if(element.lat != null) return (double) element.lat;

                return null;
            };

            Func<dynamic, double?> GetLongitude = (dynamic element) =>
            {
                if (element.center != null && element.center.lon != null)
                    return (double) element.center.lon;

                if (element.lon != null) return (double) element.lon;

                return null;
            };

            foreach (var element in data.elements)
            {
                try
                {
                    if (element.id == 369970104)
                    {
                        Console.WriteLine();
                    }

                    double? lat = GetLatitude(element);
                    double? lon = GetLongitude(element);

                    if (!lat.HasValue || !lon.HasValue) continue;

                    var tags = ((JObject) element.tags).ToObject<Dictionary<string, string>>();

                    var reststop = new Reststop
                    (
                        ID: (ulong) element.id,
                        Name: tags.GetValueOrDefault("name", null),
                        Latitude: lat.Value,
                        Longitude: lon.Value,
                        Type: tags.GetValueOrDefault("highway", "rest_area") == "services"
                                ? ReststopType.ServiceArea
                                : ReststopType.RestArea,
                        Tags: tags
                    );

                    Console.WriteLine(reststop.ID);    

                    await reststopRepository.Insert(reststop);
                    insertCount++;

                    // every 350 items, wait a second to not exceed
                    // 400 requests / second on the database
                    if (insertCount % 350 == 0)
                    {
                        await Task.Delay(1000);
                    }
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
