using Esquio;
using Esquio.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LocationToggles
{
    [DesignType(Description = "The application hostname toggle activates a feature for client instances with a hostName in the hostNames list.", FriendlyName = "Host Name")]
    [DesignTypeParameter(ParameterName = HostNames, ParameterType = EsquioConstants.SEMICOLON_LIST_PARAMETER_TYPE, ParameterDescription = "Collection of host names delimited by ';' character.")]
    public class HostNameToggle : IToggle
    {
        public const string HostNames = nameof(HostNames);
        private static readonly char[] separators = new char[] { ';' };
        private readonly IRuntimeFeatureStore _featureStore;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<HostNameToggle> _logger;

        public HostNameToggle(
            IRuntimeFeatureStore featureStore,
            IHttpContextAccessor contextAccessor,
            ILogger<HostNameToggle> logger)
        {
            _featureStore = featureStore ?? throw new ArgumentNullException(nameof(featureStore));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> IsActiveAsync(string featureName, string productName = null, CancellationToken cancellationToken = default)
        {
            var feature = await _featureStore.FindFeatureAsync(featureName, productName, cancellationToken);
            var toggle = feature.GetToggle(typeof(HostNameToggle).FullName);
            var data = toggle.GetData();

            string hostNames = data.HostNames;

            var hostName = _contextAccessor.HttpContext.Request.Host.Host;

            _logger.LogDebug($"{nameof(HostNameToggle)} is trying to verify if '{hostName}' is in the hostNames list.");

            var tokenizer = new StringTokenizer(hostNames, separators);

            foreach (var token in tokenizer)
            {
                if (token.HasValue && token.Value.Equals(hostName, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogInformation($"The hostname '{hostName}' is in the hostnames '{hostNames}' list.");

                    return true;
                }
            }

            _logger.LogInformation($"The hostname '{hostName}' is not in the hostnames list.");

            return false;
        }
    }
}
