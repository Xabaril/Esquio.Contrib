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

namespace FunctionalTests.LocationToggles.Host
{
    public class hostname_toggle_should
    {
        private const string FeatureName = nameof(FeatureName);

        [Fact]
        public void throw_if_store_is_null()
        {
            Action sut = () => new HostNameToggle(null, new StubtHttpContextAccessor(), NullLogger<HostNameToggle>.Instance);

            sut.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void throw_if_http_context_accessor_is_null()
        {
            Action sut = () => new HostNameToggle(new StubRuntimeFeatureStore((_, __) => null), null, NullLogger<HostNameToggle>.Instance);

            sut.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void throw_if_logger_is_null()
        {
            Action sut = () => new HostNameToggle(null, new StubtHttpContextAccessor(), null);

            sut.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task be_active_when_hostname_is_in_the_hostnames_list()
        {

            var toggle = Build
                .Toggle<HostNameToggle>()
                .AddOneParameter(nameof(HostNameToggle.HostNames), "localhost;domain.com")
                .Build();

            var feature = Build
                .Feature(FeatureName)
                .AddOne(toggle)
                .Build();

            var store = new StubRuntimeFeatureStore((_, __) => feature);

            var isActive = await new HostNameToggle(
                    store,
                    new StubtHttpContextAccessor(),
                    NullLogger<HostNameToggle>.Instance)
                .IsActiveAsync(FeatureName);

            isActive.Should().BeTrue();
        }

        [Fact]
        public async Task be_inactive_when_hostname_is_not_in_the_hostnames_list()
        {
            var toggle = Build
                .Toggle<HostNameToggle>()
                .AddOneParameter(nameof(HostNameToggle.HostNames), "domain.it;domain.com")
                .Build();

            var feature = Build
                .Feature(FeatureName)
                .AddOne(toggle)
                .Build();

            var store = new StubRuntimeFeatureStore((_, __) => feature);

            var isActive = await new HostNameToggle(
                    store,
                    new StubtHttpContextAccessor(),
                    NullLogger<HostNameToggle>.Instance)
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
                    context.Request.Host = new HostString("localhost", 80);
                    return context;
                }

                set => throw new NotImplementedException();
            }
        }
    }
}
