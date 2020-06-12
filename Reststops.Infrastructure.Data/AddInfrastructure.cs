using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.DependencyInjection;
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

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IDocumentClient>((IServiceProvider serviceProvider) =>
            {
                return new DocumentClient(new Uri(ACCOUNT_ENDPOINT), ACCOUNT_KEY);
            });

            // repositories
            services.AddScoped<IReststopRepository, ReststopRepository>();

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
