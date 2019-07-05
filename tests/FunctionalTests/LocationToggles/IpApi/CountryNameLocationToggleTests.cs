using FluentAssertions;
using FunctionalTests.Seedwork;
using LocationToggles;
using LocationToggles.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalTests.LocationToggles.IpApi
{
    public class ipapi_countrynamelocationtoggle_should
    {
        private const string Countries = nameof(Countries);

        [Fact]
        public async Task throw_if_store_is_nul()
        {
            var featureName = "feature-1";

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await new CountryNameLocationToggle(null, new DefaulHttpContextAccessor(), GetIpApiLocationProviderService())
                    .IsActiveAsync(featureName);
            });
        }

        [Fact]
        public async Task throw_if_http_context_accessor_is_nul()
        {
            var featureName = "feature-1";

            var toggle = Build
                  .Toggle<CountryNameLocationToggle>()
                  .AddOneParameter(Countries, "Spain;France;Germany")
                  .Build();

            var feature = Build
                .Feature(featureName)
                .AddOne(toggle)
                .Build();

            var store = new DelegatedValueFeatureStore((_, __) =>
            {
                return feature;
            });


            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await new CountryNameLocationToggle(store, null, GetIpApiLocationProviderService())
                    .IsActiveAsync(featureName);
            });
        }

        [Fact]
        public async Task throw_if_location_service_is_nul()
        {
            var featureName = "feature-1";

            var toggle = Build
                  .Toggle<CountryNameLocationToggle>()
                  .AddOneParameter(Countries, "Spain;France;Germany")
                  .Build();

            var feature = Build
                .Feature(featureName)
                .AddOne(toggle)
                .Build();

            var store = new DelegatedValueFeatureStore((_, __) =>
            {
                return feature;
            });


            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await new CountryNameLocationToggle(store, new DefaulHttpContextAccessor(), null)
                    .IsActiveAsync(featureName);
            });
        }

        [Fact]
        public async Task be_active_if_allowed_countries_include_request_origin_country()
        {
            var featureName = "feature-1";

            var toggle = Build
                  .Toggle<CountryNameLocationToggle>()
                  .AddOneParameter(Countries, "Spain;France;Germany")
                  .Build();

            var feature = Build
                .Feature(featureName)
                .AddOne(toggle)
                .Build();

            var store = new DelegatedValueFeatureStore((_, __) =>
            {
                return feature;
            });

            var isActive = await new CountryNameLocationToggle(store, new DefaulHttpContextAccessor(), GetIpApiLocationProviderService())
                .IsActiveAsync(featureName);

            isActive.Should().BeTrue();
        }

        [Fact]
        public async Task be_active_if_allowed_countries_is_origin_country()
        {
            var featureName = "feature-1";

            var toggle = Build
                  .Toggle<CountryNameLocationToggle>()
                  .AddOneParameter(Countries, "Spain")
                  .Build();

            var feature = Build
                .Feature(featureName)
                .AddOne(toggle)
                .Build();

            var store = new DelegatedValueFeatureStore((_, __) =>
            {
                return feature;
            });

            var isActive = await new CountryNameLocationToggle(store, new DefaulHttpContextAccessor(), GetIpApiLocationProviderService())
                .IsActiveAsync(featureName);

            isActive.Should().BeTrue();
        }

        [Fact]
        public async Task be_not_active_if_allowed_countries_does_not_include_request_origin_country()
        {
            var featureName = "feature-1";

            var toggle = Build
                  .Toggle<CountryNameLocationToggle>()
                  .AddOneParameter(Countries, "France")
                  .Build();

            var feature = Build
                .Feature(featureName)
                .AddOne(toggle)
                .Build();

            var store = new DelegatedValueFeatureStore((_, __) =>
            {
                return feature;
            });

            var isActive = await new CountryNameLocationToggle(store, new DefaulHttpContextAccessor(), GetIpApiLocationProviderService())
                .IsActiveAsync(featureName);

            isActive.Should().BeFalse();
        }

        private IPApiLocationProviderService GetIpApiLocationProviderService()
        {
            var logFactory = new LoggerFactory();
            var logger = logFactory.CreateLogger<IPApiLocationProviderService>();

            return new IPApiLocationProviderService(
                new DefaultHttpClientFactory(),
                logger);
        }

        private class DefaultHttpClientFactory
            : IHttpClientFactory
        {
            public HttpClient CreateClient(string name)
            {
                return new HttpClient();
            }
        }

        private class DefaulHttpContextAccessor
            : IHttpContextAccessor
        {
            public HttpContext HttpContext
            {
                get
                {
                    var context = new DefaultHttpContext();
                    context.Connection.RemoteIpAddress = IPAddress.Parse("213.97.0.42");

                    return context;
                }
                set
                {

                }
            }
        }
    }
}
