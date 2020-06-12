using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Reststops.Core.Interfaces.Services;
using Reststops.Domain.Entities;

namespace Reststops.Infrastructure.Services
{
    public class GeocodingService : IGeocodingService
    {
        private readonly string _mapboxAPIToken;
        private readonly HttpClient _httpClient;

        public GeocodingService(string mapboxAPIToken)
        {
            _mapboxAPIToken = mapboxAPIToken ??
                throw new ArgumentNullException(nameof(mapboxAPIToken));

            _httpClient = new HttpClient();
        }

        #region Public Methods

        public async Task<ForwardGeocoding> GetForwardGeocoding(string text)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(
                $"https://api.mapbox.com/geocoding/v5/mapbox.places/{WebUtility.UrlEncode(text)}.json?" +
                $"autocomplete=true&access_token={_mapboxAPIToken}"
            );

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ForwardGeocoding>(body);
            }

            return null;
        }

        #endregion
    }
}
