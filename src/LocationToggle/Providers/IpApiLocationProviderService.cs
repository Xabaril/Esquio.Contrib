using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LocationToggles.Providers
{
    public class IPApiLocationProviderService
       : ILocationProviderService
    {
        const string IP_API_LOCATION_SERVICE = nameof(IP_API_LOCATION_SERVICE);
        const string BASE_ADDRESS = "http://ip-api.com/json/";

        static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IPApiLocationProviderService> _logger;

        public IPApiLocationProviderService(IHttpClientFactory httpClientFactory, ILogger<IPApiLocationProviderService> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


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
            try
            {
                var httpClient = _httpClientFactory
                    .CreateClient(IP_API_LOCATION_SERVICE);

                var response = await httpClient.GetAsync($"{BASE_ADDRESS}{ipaddress}", cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();

                    return await JsonSerializer.DeserializeAsync<IPApiData>(
                        utf8Json: stream,
                        options: _serializerOptions,
                        cancellationToken);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "IpApiLocationProviderService throw an exception when trying to get country information");
            }

            return default;
        }

        public class IPApiData
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
