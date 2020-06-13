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
            IEnumerable<Coordinate> coordinates
        )
        {
            string coordinatesUrl = string.Join(
                ';',
                coordinates.Select(c => ToMapboxString(c))
            );

            HttpResponseMessage response = await _httpClient.GetAsync(
                $"https://api.mapbox.com/directions/v5/{PROFILE}/{coordinatesUrl}" + 
                $"?geometries=geojson&access_token={_mapboxAPIToken}"
            );

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DirectionsRoute>(body);
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
