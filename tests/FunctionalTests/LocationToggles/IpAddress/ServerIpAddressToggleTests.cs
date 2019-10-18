using Esquio.Abstractions;
using Esquio.Model;
using Esquio.Toggles.Http;
using FluentAssertions;
using FunctionalTests.Seedwork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalTests.LocationToggles.IpAddress
{
    public class server_ip_address_toggle_should
    {
        private const string FeatureName = nameof(FeatureName);

        [Fact]
        public void throw_if_store_is_null()
        {
            Action sut = () => new ServerIpAddressToggle(null, new StubtHttpContextAccessor(), NullLogger<ServerIpAddressToggle>.Instance);

            sut.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void throw_if_http_context_accessor_is_null()
        {
            Action sut = () => new ServerIpAddressToggle(new StubRuntimeFeatureStore((_, __) => null), null, NullLogger<ServerIpAddressToggle>.Instance);

            sut.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void throw_if_logger_is_null()
        {
            Action sut = () => new HostNameToggle(null, new StubtHttpContextAccessor(), null);

            sut.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task be_active_when_ip_address_is_in_the_ip_addresses_list()
        {

            var toggle = Build
                .Toggle<ServerIpAddressToggle>()
                .AddOneParameter(nameof(ServerIpAddressToggle.IpAddresses), "143.24.20.35;143.24.20.36")
                .Build();

            var feature = Build
                .Feature(FeatureName)
                .AddOne(toggle)
                .Build();

            var store = new StubRuntimeFeatureStore((_, __) => feature);

            var isActive = await new ServerIpAddressToggle(
                    store,
                    new StubtHttpContextAccessor(),
                    NullLogger<ServerIpAddressToggle>.Instance)
                .IsActiveAsync(FeatureName);

            isActive.Should().BeTrue();
        }

        [Fact]
        public async Task be_inactive_when_ip_address_is_in_the_ip_addresses_list()
        {
            var toggle = Build
                .Toggle<ServerIpAddressToggle>()
                .AddOneParameter(nameof(ServerIpAddressToggle.IpAddresses), "134.24.21.35;134.24.21.36")
                .Build();

            var feature = Build
                .Feature(FeatureName)
                .AddOne(toggle)
                .Build();

            var store = new StubRuntimeFeatureStore((_, __) => feature);

            var isActive = await new ServerIpAddressToggle(
                    store,
                    new StubtHttpContextAccessor(),
                    NullLogger<ServerIpAddressToggle>.Instance)
                .IsActiveAsync(FeatureName);

            isActive.Should().BeFalse();
        }

        private class StubRuntimeFeatureStore : IRuntimeFeatureStore
        {
            private readonly Func<string, string, Feature> function;

            public StubRuntimeFeatureStore(Func<string, string, Feature> function)
            {
                this.function = function;
            }

            public Task<Feature> FindFeatureAsync(string featureName, string productName = null, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(function(featureName, productName));
            }
        }

        private class StubtHttpContextAccessor : IHttpContextAccessor
        {
            public HttpContext HttpContext
            {
                get
                {
                    var context = new DefaultHttpContext();
                    context.Connection.LocalIpAddress = new System.Net.IPAddress(0x2414188f); //IP 143.24.20.36
                    return context;
                }

                set => throw new NotImplementedException();
            }
        }
    }
}
