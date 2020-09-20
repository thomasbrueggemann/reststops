using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Reststops.Core.Entities;
using Reststops.Core.Interfaces.Repositories;
using Reststops.Core.Interfaces.Services;
using Reststops.Infrastructure.Repositories;
using Reststops.Infrastructure.Services;

namespace Reststops.Infrastructure.Data
{
    public static class AddInfrastructureServices
    {
        private static readonly string ACCOUNT_ENDPOINT = Environment.GetEnvironmentVariable("COSMOSDB_ACCOUNT_ENDPOINT");
        private static readonly string ACCOUNT_KEY = Environment.GetEnvironmentVariable("COSMOSDB_ACCOUNT_KEY");
        private static readonly string MAPBOX_TOKEN = Environment.GetEnvironmentVariable("MAPBOX_TOKEN");
        private static readonly string ATLAS_CONN_STRING = Environment.GetEnvironmentVariable("ATLAS_CONN_STRING");
        
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IDocumentClient>((IServiceProvider serviceProvider) =>
            {
                return new DocumentClient(new Uri(ACCOUNT_ENDPOINT), ACCOUNT_KEY);
            });
            
            services.AddSingleton<IMongoClient>((IServiceProvider serviceProvider) =>
            {
                return new MongoClient(ATLAS_CONN_STRING);
            });

            // repositories
            services.AddScoped<IReststopRepository, ReststopMongoDBRepository>();

            // services
            services.AddScoped<INavigationService>((IServiceProvider serviceProvider) =>
            {
                return new NavigationService(MAPBOX_TOKEN);
            });

            services.AddScoped<IGeocodingService>((IServiceProvider serviceProvider) =>
            {
                return new GeocodingService(MAPBOX_TOKEN);
            });

            return services;
        }
    }
}
