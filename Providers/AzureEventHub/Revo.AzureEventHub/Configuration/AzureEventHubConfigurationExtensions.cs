using System;
using Revo.Core.Configuration;
using Revo.AzureEventHub.EventStores;

namespace Revo.AzureEventHub.Configuration
{
    /// <summary>
    /// Configuration extensions for Azure Event Hub provider.
    /// </summary>
    public static class AzureEventHubConfigurationExtensions
    {
        /// <summary>
        /// Configures Azure Event Hub as the event store.
        /// </summary>
        /// <param name="configuration">The Revo configuration.</param>
        /// <param name="options">Configuration options for Azure Event Hub.</param>
        /// <returns>The configuration for chaining.</returns>
        public static IRevoConfiguration UseAzureEventHubEventStore(
            this IRevoConfiguration configuration,
            Action<AzureEventHubEventStoreConfiguration> options)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var config = new AzureEventHubEventStoreConfiguration();
            options(config);

            configuration.ConfigureKernel(c =>
            {
                c.Bind<AzureEventHubEventStoreConfiguration>()
                    .ToConstant(config)
                    .InSingletonScope();

                c.Bind<IEventStore>()
                    .To<AzureEventHubEventStore>()
                    .InTaskScope();

                c.Bind<IExternalEventStore>()
                    .To<AzureEventHubExternalEventStore>()
                    .InTaskScope();
            });

            return configuration;
        }

        /// <summary>
        /// Configures Azure Event Hub as the event store with default settings.
        /// </summary>
        /// <param name="configuration">The Revo configuration.</param>
        /// <param name="connectionString">Azure Event Hub connection string.</param>
        /// <param name="eventHubName">Azure Event Hub name.</param>
        /// <returns>The configuration for chaining.</returns>
        public static IRevoConfiguration UseAzureEventHubEventStore(
            this IRevoConfiguration configuration,
            string connectionString,
            string eventHubName)
        {
            return configuration.UseAzureEventHubEventStore(options =>
            {
                options.ConnectionString = connectionString;
                options.EventHubName = eventHubName;
            });
        }
    }
}
