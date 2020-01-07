using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Esquio.Toggles.GeoLocation
{
    internal class IPApiLocationProviderService
    {
        static HttpClient _httpClient = new HttpClient();

        const string IP_API_LOCATION_SERVICE = nameof(IP_API_LOCATION_SERVICE);
        const string BASE_ADDRESS = "http://ip-api.com/json/";

        static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };

        public async Task<string> GetCountryName(string ipaddress, CancellationToken cancellationToken = default)
        {
            var data = await GetIpDataFrom(ipaddress, cancellationToken);

            return data?.Country;
        }

        public async Task<string> GetCountryCode(string ipaddress, CancellationToken cancellationToken = default)
        {
            var data = await GetIpDataFrom(ipaddress, cancellationToken);

            return data?.CountryCode;
        }

        private async Task<IPApiData> GetIpDataFrom(string ipaddress, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{BASE_ADDRESS}{ipaddress}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();

                return await JsonSerializer.DeserializeAsync<IPApiData>(
                    utf8Json: stream,
                    options: _serializerOptions,
                    cancellationToken);
            }

            return default;
        }

        internal class IPApiData
        {
            public string Query { get; set; }
            public string Status { get; set; }
            public string Country { get; set; }
            public string CountryCode { get; set; }
            public string Region { get; set; }
            public string RegionName { get; set; }
            public string City { get; set; }
            public string Zip { get; set; }
            public float Lat { get; set; }
            public float Lon { get; set; }
            public string Timezone { get; set; }
            public string Isp { get; set; }
            public string Org { get; set; }
            public string _as { get; set; }
        }
    }
}
