using Esquio.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Esquio.Toggles.GeoLocation
{
    [DesignType(Description = "Toggle that is active depending on Country codes for the request ip location using IpStack service.", FriendlyName = "IpStack  Country Code")]
    [DesignTypeParameter(ParameterName = Countries, ParameterType = EsquioConstants.SEMICOLON_LIST_PARAMETER_TYPE, ParameterDescription = "Collection of country codes delimited by ';' character.")]
    [DesignTypeParameter(ParameterName = AccessKey, ParameterType = EsquioConstants.STRING_PARAMETER_TYPE, ParameterDescription = "A valid  IpStack Access Key.")]
    public class IpStackCountryCodeToggle
      : IToggle
    {
        const string Countries = nameof(Countries);
        const string AccessKey = nameof(AccessKey);

        private readonly IpStackLocationProviderService _locationProviderService = new IpStackLocationProviderService();

        private readonly IRuntimeFeatureStore _featureStore;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IpStackCountryCodeToggle(IRuntimeFeatureStore featureStore, IHttpContextAccessor httpContextAccessor)
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
            string accessKey = data.AccessKey;

            var currentCountry = await _locationProviderService
                .GetCountryCode(GetRemoteIpAddress(), accessKey);

            if (allowedCountries != null
                &&
                currentCountry != null
                &&
                accessKey != null)
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
