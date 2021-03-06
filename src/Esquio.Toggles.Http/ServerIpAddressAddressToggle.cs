﻿using Esquio;
using Esquio.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Esquio.Toggles.Http
{
    [DesignType(Description = "The server IP address toggle activates a feature for ip addresses defined in the IP list.", FriendlyName = "Server IP")]
    [DesignTypeParameter(ParameterName = IpAddresses, ParameterType = EsquioConstants.SEMICOLON_LIST_PARAMETER_TYPE, ParameterDescription = "Collection of IP addresses delimited by ';' character.")]
    public class ServerIpAddressToggle : IToggle
    {
        public const string IpAddresses = nameof(IpAddresses);

        private readonly IRuntimeFeatureStore _featureStore;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<ServerIpAddressToggle> _logger;

        public ServerIpAddressToggle(
            IRuntimeFeatureStore featureStore,
            IHttpContextAccessor contextAccessor,
            ILogger<ServerIpAddressToggle> logger)
        {
            _featureStore = featureStore ?? throw new ArgumentNullException(nameof(featureStore));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> IsActiveAsync(string featureName, string productName = null, CancellationToken cancellationToken = default)
        {
            var feature = await _featureStore.FindFeatureAsync(featureName, productName, cancellationToken);
            var toggle = feature.GetToggle(typeof(ServerIpAddressToggle).FullName);
            var data = toggle.GetData();
            var ipAddress = _contextAccessor.HttpContext.Connection.LocalIpAddress;
            var bytes = ipAddress.GetAddressBytes();
            string ipAddresses = data.IpAddresses;

            _logger.LogDebug($"{nameof(ServerIpAddressToggle)} is trying to verify if '{ipAddress}' is in the IP list.");

            var tokenizer = new StringTokenizer(ipAddresses, EsquioConstants.DEFAULT_SPLIT_SEPARATOR);

            foreach (var token in tokenizer)
            {
                if (token.HasValue
                    && IPAddress.TryParse(token, out IPAddress address)
                    && address.GetAddressBytes().SequenceEqual(bytes))
                {
                    _logger.LogInformation($"The server IP address '{ipAddress}' is in the IP '{ipAddresses}' list.");

                    return true;
                }
            }

            _logger.LogInformation($"The server IP address '{ipAddress}' is not in the IP list.");

            return false;
        }
    }
}
