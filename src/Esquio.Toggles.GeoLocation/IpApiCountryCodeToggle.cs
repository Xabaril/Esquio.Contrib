using Esquio;
using Esquio.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Esquio.Toggles.GeoLocation
{
    [DesignType(Description = "Toggle that is active depending on Country code for the request ip location using IpApi service.", FriendlyName = "IpApi Country Code")]
    [DesignTypeParameter(ParameterName = Countries, ParameterType = EsquioConstants.SEMICOLON_LIST_PARAMETER_TYPE, ParameterDescription = "Collection of country codes delimited by ';' character.")]
    public class IpApiCountryCodeToggle
      : IToggle
    {
        const string Countries = nameof(Countries);

        private readonly IPApiLocationProviderService _locationProviderService = new IPApiLocationProviderService();

        private readonly IRuntimeFeatureStore _featureStore;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IpApiCountryCodeToggle(IRuntimeFeatureStore featureStore, IHttpContextAccessor httpContextAccessor)
        {
            _featureStore = featureStore ?? throw new ArgumentNullException();
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<bool> IsActiveAsync(string featureName, string productName = null, CancellationToken cancellationToken = default)
        {
            var feature = await _featureStore.FindFeatureAsync(featureName, productName);
            var toggle = feature.GetToggle(this.GetType().FullName);
            var data = toggle.GetData();

            string allowedCountries = data.Countries;
            var currentCountry = await _locationProviderService
                .GetCountryCode(GetRemoteIpAddress());

            if (allowedCountries != null
                &&
                currentCountry != null)
            {
                var tokenizer = new StringTokenizer(allowedCountries, EsquioConstants.DEFAULT_SPLIT_SEPARATOR);

                return tokenizer.Contains(currentCountry, StringSegmentComparer.OrdinalIgnoreCase);
            }

            return false;
        }

        string GetRemoteIpAddress()
        {
            const string HEADER_X_FORWARDED_FOR = "X-Forwarded-For";

            string remoteIpAddress = _httpContextAccessor.HttpContext
                .Connection
                .RemoteIpAddress
                .MapToIPv4()
                .ToString();

            if (_httpContextAccessor.HttpContext
                .Request
                .Headers
                .ContainsKey(HEADER_X_FORWARDED_FOR))
            {
                remoteIpAddress = _httpContextAccessor.HttpContext
                    .Request.Headers[HEADER_X_FORWARDED_FOR];
            }

            return remoteIpAddress;
        }
    }
}
