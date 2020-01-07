using Esquio.Toggles.GeoLocation;
using FluentAssertions;
using FunctionalTests.Seedwork;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalTests.LocationToggles.IpStack
{
    public class continentlocationtoggle_should
    {
        private const string Continents = nameof(Continents);
        private const string AccessKey = nameof(AccessKey);

        [Fact]
        public async Task throw_if_store_is_null()
        {
            var featureName = "feature-1";

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await new IpStackContinentNameToggle(null, new DefaulHttpContextAccessor())
                    .IsActiveAsync(featureName);
            });
        }

        [Fact]
        public async Task throw_if_http_context_accessor_is_null()
        {
            var featureName = "feature-1";

            var toggle = Build
                  .Toggle<IpStackContinentNameToggle>()
                  .AddOneParameter(Continents, "Europe")
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
                await new IpStackContinentNameToggle(store, null)
                    .IsActiveAsync(featureName);
            });
        }

        [Fact]
        public async Task be_active_if_allowed_zip_include_request_origin_zip()
        {
            var featureName = "feature-1";

            var toggle = Build
                  .Toggle<IpStackContinentNameToggle>()
                  .AddOneParameter(Continents, "Europe;Asia")
                  .AddOneParameter(AccessKey, IpStackConstants.AccessKey)
                  .Build();

            var feature = Build
                .Feature(featureName)
                .AddOne(toggle)
                .Build();

            var store = new DelegatedValueFeatureStore((_, __) =>
            {
                return feature;
            });

            var isActive = await new IpStackContinentNameToggle(store, new DefaulHttpContextAccessor())
                .IsActiveAsync(featureName);

            isActive.Should().BeTrue();
        }

        [Fact]
        public async Task be_active_if_allowed_zip_is_origin_zipcode()
        {
            var featureName = "feature-1";

            var toggle = Build
                  .Toggle<IpStackContinentNameToggle>()
                  .AddOneParameter(Continents, "Europe")
                  .AddOneParameter(AccessKey, IpStackConstants.AccessKey)
                  .Build();

            var feature = Build
                .Feature(featureName)
                .AddOne(toggle)
                .Build();

            var store = new DelegatedValueFeatureStore((_, __) =>
            {
                return feature;
            });

            var isActive = await new IpStackContinentNameToggle(store, new DefaulHttpContextAccessor())
                .IsActiveAsync(featureName);

            isActive.Should().BeTrue();
        }

        [Fact]
        public async Task be_not_active_if_allowed_countries_does_not_include_request_origin_country()
        {
            var featureName = "feature-1";

            var toggle = Build
                  .Toggle<IpStackContinentNameToggle>()
                  .AddOneParameter(Continents, "America")
                  .AddOneParameter(AccessKey, IpStackConstants.AccessKey)
                  .Build();

            var feature = Build
                .Feature(featureName)
                .AddOne(toggle)
                .Build();

            var store = new DelegatedValueFeatureStore((_, __) =>
            {
                return feature;
            });

            var isActive = await new IpStackContinentNameToggle(store, new DefaulHttpContextAccessor())
                .IsActiveAsync(featureName);

            isActive.Should().BeFalse();
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
