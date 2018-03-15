using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Praxair.Web.Base.Interfaces;

namespace Praxair.Web.Base.Localization.Json
{
    /// <summary>
    /// Extension methods for adding localization servics to the DI container.
    /// </summary>
    public static class LocalizationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required for application localization.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return AddJsonLocalization(services, setupAction: null);
        }

        /// <summary>
        /// Adds services required for application localization.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="setupAction">An action to configure the <see cref="LocalizationOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddJsonLocalization(
            this IServiceCollection services,
            Action<LocalizationOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            var serviceProvider = services.BuildServiceProvider();
            var stringLocalizerFactory = new StringLocalizerFactory(
                serviceProvider.GetRequiredService<IFileProvider>(),
                serviceProvider.GetRequiredService<ILoggerFactory>(),
                serviceProvider.GetRequiredService<IMemoryCache>(),
                serviceProvider.GetRequiredService<IOptions<LocalizationOptions>>());


            services.AddSingleton<IStringLocalizerFactory>(stringLocalizerFactory);
            services.AddSingleton<IExtendedStringLocalizerFactory>(stringLocalizerFactory);

            return services;
        }

    }
}
