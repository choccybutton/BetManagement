using System;
using Microsoft.Extensions.DependencyInjection;
using FootballAPIWrapper.Configuration;
using FootballAPIWrapper.Usage;

namespace FootballAPIWrapper.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Football API services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The Football API configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddFootballApi(this IServiceCollection services, FootballApiConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            configuration.Validate();

            services.AddSingleton(configuration);
            services.AddSingleton<IUsageTracker, UsageTracker>();
            services.AddHttpClient<IFootballApiClient, FootballApiClient>();

            return services;
        }

        /// <summary>
        /// Adds Football API services to the service collection with configuration action
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configureOptions">Action to configure the Football API options</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddFootballApi(this IServiceCollection services, Action<FootballApiConfiguration> configureOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            var configuration = new FootballApiConfiguration();
            configureOptions(configuration);

            return AddFootballApi(services, configuration);
        }
    }
}