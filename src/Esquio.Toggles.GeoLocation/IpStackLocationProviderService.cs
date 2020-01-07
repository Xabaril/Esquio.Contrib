using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Esquio.Toggles.GeoLocation
{
    internal class IpStackLocationProviderService
    {
        static HttpClient _httpClient = new HttpClient();

        const string IPSTACK_API_LOCATION_SERVICE = nameof(IPSTACK_API_LOCATION_SERVICE);
        const string BASE_ADDRESS = "http://api.ipstack.com/";

        static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };

        public async Task<string> GetCountryName(string ipaddress, string accessKey, CancellationToken cancellationToken = default)
        {
            var data = await GetIpStackDataFrom(ipaddress, accessKey, cancellationToken);

            return data?.CountryName;
        }

        public async Task<string> GetCityName(string ipaddress, string accessKey, CancellationToken cancellationToken = default)
        {
            var data = await GetIpStackDataFrom(ipaddress, accessKey, cancellationToken);

            return data?.City;
        }

        public async Task<string> GetZipCode(string ipaddress, string accessKey, CancellationToken cancellationToken = default)
        {
            var data = await GetIpStackDataFrom(ipaddress, accessKey, cancellationToken);

            return data?.Zip;
        }

        public async Task<string> GetContinentName(string ipaddress, string accessKey, CancellationToken cancellationToken = default)
        {
            var data = await GetIpStackDataFrom(ipaddress, accessKey, cancellationToken);

            return data?.ContinentName;
        }

        public async Task<string> GetContinentCode(string ipaddress, string accessKey, CancellationToken cancellationToken = default)
        {
            var data = await GetIpStackDataFrom(ipaddress, accessKey, cancellationToken);

            return data?.ContinentCode;
        }

        public async Task<string> GetCountryCode(string ipaddress, string accessKey, CancellationToken cancellationToken = default)
        {
            var data = await GetIpStackDataFrom(ipaddress, accessKey, cancellationToken);

            return data?.CountryCode;
        }

        private async Task<IpStackData> GetIpStackDataFrom(string ipaddress, string accessKey, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{BASE_ADDRESS}{ipaddress}?access_key={accessKey}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();

                return await JsonSerializer.DeserializeAsync<IpStackData>(
                    utf8Json: stream,
                    options: _serializerOptions,
                    cancellationToken);
            }

            return default;
        }

        private class IpStackData
        {
            public string Ip { get; set; }
            public string HostName { get; set; }
            public string Type { get; set; }
            [JsonPropertyName("continent_code")]
            public string ContinentCode { get; set; }
            [JsonPropertyName("continent_name")]
            public string ContinentName { get; set; }
            [JsonPropertyName("country_code")]
            public string CountryCode { get; set; }
            [JsonPropertyName("country_name")]
            public string CountryName { get; set; }
            [JsonPropertyName("region_code")]
            public string RegionCode { get; set; }
            [JsonPropertyName("region_name")]
            public string RegionName { get; set; }
            public string City { get; set; }
            public string Zip { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
        }
    }
}
