using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StsGateway.Contracts;
using StsGateway.Extensions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace StsGatewayTest
{
    public class StsGatewayUnitTest
    {
        [Fact(DisplayName = "Given that I don't have cache and my client id and secret key be correct. When executing access token, be prompted. Then the access token should be created.")]
        public async Task SuccessCase()
        {
            var server = WireMockServer.Start();
            server.Given(Request.Create().UsingPost())
                .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new
                {
                    access_token = "MTQ0NjJkZmQ5OTM2NDE1ZTZjNGZmZjI3",
                    token_type = "Bearer",
                    expires_in = 5,
                    refresh_token = "IwOGYzYTlmM2YxOTQ5MGE3YmNmMDFkNTVk",
                    scope = "create",
                    active = true
                }));

            ServiceCollection services = new ServiceCollection();
            services.AddStsGateway((sp, config) =>
            {
                config.ClientId = "test";
                config.ClientSecret = "test";
                config.GrantType = "test";
                config.RequestUri = new Uri(server.Url!);
            });

            var serviceProvider = services.BuildServiceProvider();

            var stsGateway = serviceProvider.GetService<IStsGateway>();
            var accessToken = await stsGateway!.GetAccessTokenAsync();

            accessToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "Given that I have memory cache but is empty and my client id and secret key be correct. When executing access token, be prompted. Then the access token should be created.")]
        public async Task SuccessCaseWithMemoryCache()
        {
            var server = WireMockServer.Start();
            server.Given(Request.Create().UsingPost())
                .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new
                {
                    access_token = "MTQ0NjJkZmQ5OTM2NDE1ZTZjNGZmZjI3",
                    token_type = "Bearer",
                    expires_in = 5,
                    refresh_token = "IwOGYzYTlmM2YxOTQ5MGE3YmNmMDFkNTVk",
                    scope = "create",
                    active = true
                }));

            ServiceCollection services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddStsGateway((sp, config) =>
            {
                config.ClientId = "test";
                config.ClientSecret = "test";
                config.GrantType = "test";
                config.RequestUri = new Uri(server.Url!);
                config.CacheType = StsGateway.StsGatewayOptions.CacheTypeEnum.MemoryCache;
            });

            var serviceProvider = services.BuildServiceProvider();

            var stsGateway = serviceProvider.GetService<IStsGateway>();
            var accessToken = await stsGateway!.GetAccessTokenAsync();

            accessToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "Given that I don't have cache and my client id and secret key be incorrect. When executing access token, be prompted. Then the access token should'nt be created.")]
        public async Task FailCaseOne()
        {
            var server = WireMockServer.Start();
            server.Given(Request.Create().UsingPost())
                .RespondWith(Response.Create().WithStatusCode(401));

            ServiceCollection services = new ServiceCollection();
            services.AddStsGateway((sp, config) =>
            {
                config.ClientId = "test";
                config.ClientSecret = "test";
                config.GrantType = "test";
                config.RequestUri = new Uri(server.Url!);
            });

            var serviceProvider = services.BuildServiceProvider();

            var stsGateway = serviceProvider.GetService<IStsGateway>();
            var accessToken = await stsGateway!.GetAccessTokenAsync();

            accessToken.Should().BeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "Given that I have memory cache but is empty and my client id and secret key be incorrect. When executing access token, be prompted. Then the access token should'nt be created.")]
        public async Task FailCaseOneWithMemoryCache()
        {
            var server = WireMockServer.Start();
            server.Given(Request.Create().UsingPost())
                .RespondWith(Response.Create().WithStatusCode(401));

            ServiceCollection services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddStsGateway((sp, config) =>
            {
                config.ClientId = "test";
                config.ClientSecret = "test";
                config.GrantType = "test";
                config.RequestUri = new Uri(server.Url!);
            });

            var serviceProvider = services.BuildServiceProvider();

            var stsGateway = serviceProvider.GetService<IStsGateway>();
            var accessToken = await stsGateway!.GetAccessTokenAsync();

            accessToken.Should().BeNullOrWhiteSpace();
        }
    }
}