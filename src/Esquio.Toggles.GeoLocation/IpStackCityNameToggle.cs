using Esquio.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Esquio.Toggles.GeoLocation
{
    [DesignType(Description = "Toggle that is active depending on city names for the request ip location using IpStack service.", FriendlyName = "IpStack City Name")]
    [DesignTypeParameter(ParameterName = Cities, ParameterType = EsquioConstants.SEMICOLON_LIST_PARAMETER_TYPE, ParameterDescription = "Collection of city names delimited by ';' character.")]
    [DesignTypeParameter(ParameterName = AccessKey, ParameterType = EsquioConstants.STRING_PARAMETER_TYPE, ParameterDescription = "A valid  IpStack Access Key.")]
    public class IpStackCityNameToggle
      : IToggle
    {
        const string Cities = nameof(Cities);
        const string AccessKey = nameof(AccessKey);

        private readonly IpStackLocationProviderService _locationProviderService = new IpStackLocationProviderService();

        private readonly IRuntimeFeatureStore _featureStore;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IpStackCityNameToggle(IRuntimeFeatureStore featureStore, IHttpContextAccessor httpContextAccessor)
        {
            _featureStore = featureStore ?? throw new ArgumentNullException();
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<bool> IsActiveAsync(string featureName, string productName = null, CancellationToken cancellationToken = default)
        {
            var feature = await _featureStore.FindFeatureAsync(featureName, productName);
            var toggle = feature.GetToggle(this.GetType().FullName);
            var data = toggle.GetData();

            string allowedCities = data.Cities;
            string accessKey = data.AccessKey;

            var currentCity = await _locationProviderService
                .GetCityName(GetRemoteIpAddress(), accessKey);

            if (allowedCities != null
                &&
                currentCity != null)
            {
                var tokenizer = new StringTokenizer(allowedCities, EsquioConstants.DEFAULT_SPLIT_SEPARATOR);

                return tokenizer.Contains(currentCity, StringSegmentComparer.OrdinalIgnoreCase);
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
