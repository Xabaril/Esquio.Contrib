using Esquio;
using Esquio.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace LocationToggles
{
    [DesignType(Description = "The IP address toggle activates a feature for ip addresses defined in the IP list.")]
    [DesignTypeParameter(ParameterName = IpAddresses, ParameterType = EsquioConstants.SEMICOLON_LIST_PARAMETER_TYPE, ParameterDescription = "Collection of IP addresses delimited by ';' character.")]
    public class IpAddressToggle : IToggle
    {
        public const string IpAddresses = nameof(IpAddresses);
        private static readonly char [] separators = new char[] { ';' };
        private readonly IRuntimeFeatureStore _featureStore;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<IpAddressToggle> _logger;

        public IpAddressToggle(
            IRuntimeFeatureStore featureStore, 
            IHttpContextAccessor contextAccessor,
            ILogger<IpAddressToggle> logger)
        {
            _featureStore = featureStore ?? throw new ArgumentNullException(nameof(featureStore));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> IsActiveAsync(string featureName, string productName = null, CancellationToken cancellationToken = default)
        {
            var feature = await _featureStore.FindFeatureAsync(featureName, productName, cancellationToken);
            var toggle = feature.GetToggle(typeof(IpAddressToggle).FullName);
            var data = toggle.GetData();
            var ipAddress = _contextAccessor.HttpContext.Connection.RemoteIpAddress;
            var bytes = ipAddress.GetAddressBytes();
            string ipAddresses = data.IpAddresses;
            
            _logger.LogDebug($"{nameof(IpAddressToggle)} is trying to verify if '{ipAddress}' is in the IP list.");

            var tokenizer = new StringTokenizer(ipAddresses, separators);

            foreach (var token in tokenizer)
            {
                if (token.HasValue 
                    && IPAddress.TryParse(token, out IPAddress address) 
                    && address.GetAddressBytes().SequenceEqual(bytes))
                {
                    _logger.LogInformation($"The IP address '{ipAddress}' is in the IP '{ipAddresses}' list.");

                    return true;
                }
            }

            _logger.LogInformation($"The IP address '{ipAddress}' is not in the IP list.");

            return false;
        }
    }
}
