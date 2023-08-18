using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StsGateway.Contracts;
using System;

namespace StsGateway.Extensions
{
    public static class ServiceCollectionExtension
    {

        public static IServiceCollection AddStsGateway(this IServiceCollection services, Action<IServiceProvider, StsGatewayOptions> setupAction)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            IServiceProvider _sp = services.BuildServiceProvider();
            StsGatewayOptions _options = new StsGatewayOptions();

            setupAction(_sp, _options);

            services.AddOptions();
            services.Configure<StsGatewayOptions>(config =>
            {
                config.ClientId = _options.ClientId;
                config.CacheType = _options.CacheType;
                config.ClientSecret = _options.ClientSecret;
                config.GrantType = _options.GrantType;
                config.RequestUri = _options.RequestUri;
            });

            services.AddSingleton<IStsGateway, StsGateway>();

            return services;
        }

        public static IServiceCollection AddStsGateway(this IServiceCollection services, Action<StsGatewayOptions> setupAction)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            services.AddOptions();
            services.Configure(setupAction);

            services.AddSingleton<IStsGateway, StsGateway>();

            return services;
        }
    }
}
