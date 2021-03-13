using System;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Reststops.Core.Interfaces.Repositories;
using Reststops.Core.Interfaces.Services;
using Reststops.Infrastructure.Repositories;
using Reststops.Infrastructure.Services;

namespace Reststops.Infrastructure.Data
{
    public static class AddInfrastructureServices
    {
        private static readonly string MAPBOX_TOKEN = Environment.GetEnvironmentVariable("MAPBOX_TOKEN");
        private static readonly string ATLAS_CONN_STRING = Environment.GetEnvironmentVariable("ATLAS_CONN_STRING");

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IMongoClient>(_ =>
            {
                return new MongoClient(ATLAS_CONN_STRING);
            });

            services.AddScoped<IReststopRepository, ReststopMongoDBRepository>();
            
            services.AddScoped<INavigationService>(_ =>
            {
                return new NavigationService(MAPBOX_TOKEN);
            });

            services.AddScoped<IGeocodingService>(_ =>
            {
                return new GeocodingService(MAPBOX_TOKEN);
            });

            return services;
        }
    }
}
