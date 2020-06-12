using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Reststops.Core.Interfaces.Services;
using Reststops.Domain.Entities;

namespace Reststops.Infrastructure.Services
{
    public class NavigationService : INavigationService
    {
        private readonly string _mapboxAPIToken;
        private readonly HttpClient _httpClient;

        private const string PROFILE = "mapbox/driving";

        public NavigationService(string mapboxAPIToken)
        {
            _mapboxAPIToken = mapboxAPIToken ??
                throw new ArgumentNullException(nameof(mapboxAPIToken));

            _httpClient = new HttpClient();
        }

        #region Public Methods

        public async Task<DirectionsRoute> GetDirections(
            Coordinate startCoordinate,
            Coordinate endCoordinate
        )
        {
            string coordinates = $"{ToMapboxString(startCoordinate)};{ToMapboxString(endCoordinate)}";

            HttpResponseMessage response = await _httpClient.GetAsync(
                $"https://api.mapbox.com/directions/v5/{PROFILE}/{coordinates}" + 
                $"?geometries=geojson&access_token={_mapboxAPIToken}"
            );

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DirectionsRoute>(body);
            }

            return null;
        }

        public async Task<DirectionsMatrix> GetMatrix(Coordinate source, IEnumerable<Coordinate> destinations)
        {
            var coordinates = new List<Coordinate>() { source };
            coordinates.AddRange(destinations);

            string destinationIndices = string.Join(';', Enumerable.Range(1, destinations.Count()));

            HttpResponseMessage response = await _httpClient.GetAsync(
                $"https://api.mapbox.com/directions-matrix/v1/{PROFILE}/{ToMapboxString(coordinates)}?" +
                $"sources=0&destinations={destinationIndices}&fallback_speed=80&" +
                $"annotations=duration,distance&access_token={_mapboxAPIToken}"
            );

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DirectionsMatrix>(body);
            }

            return null;
        }

        #endregion

        #region Private Methods

        private static string ToMapboxString(Coordinate coord)
            => $"{coord.X},{coord.Y}";

        private static string ToMapboxString(IEnumerable<Coordinate> coords)
            => string.Join(';', coords.Select(c => ToMapboxString(c)));

        #endregion
    }
}
