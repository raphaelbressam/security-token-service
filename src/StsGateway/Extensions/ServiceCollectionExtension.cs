using Microsoft.Extensions.DependencyInjection;
using StsGateway.Contracts;
using System;

namespace StsGateway.Extensions
{
    public static class ServiceCollectionExtension
    {
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
