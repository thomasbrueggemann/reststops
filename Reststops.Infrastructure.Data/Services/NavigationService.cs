using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Reststops.Core.Interfaces.Services;
using Reststops.Core.Entities;

namespace Reststops.Infrastructure.Services
{
    public class NavigationService : INavigationService
    {
        private readonly string _mapboxApiToken;
        private readonly HttpClient _httpClient;

        private const string Profile = "mapbox/driving";

        public NavigationService(string mapboxAPIToken)
        {
            _mapboxApiToken = mapboxAPIToken ??
                throw new ArgumentNullException(nameof(mapboxAPIToken));

            _httpClient = new HttpClient();
        }

        #region Public Methods

        public async Task<DirectionsRoute> GetDirections(IEnumerable<Coordinate> coordinates)
        {
            var coordinatesUrl = string.Join(
                ';',
                coordinates.Select(ToMapboxString)
            );

            var url = $"https://api.mapbox.com/directions/v5/{Profile}/{coordinatesUrl}" +
                      $"?geometries=geojson&access_token={_mapboxApiToken}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DirectionsRoute>(body);
            }

            return null;
        }

        #endregion

        #region Private Methods

        private static string ToMapboxString(Coordinate coordinate)
            => $"{coordinate.X},{coordinate.Y}";

        private static string ToMapboxString(IEnumerable<Coordinate> coordinates)
            => string.Join(';', coordinates.Select(ToMapboxString));

        #endregion
    }
}
