using System;
using Revo.Core.Core;
using Revo.DependencyInjection.Core;
using Revo.Infrastructure.EventStores;
using Revo.AzureEventHub.Configuration;
using Revo.AzureEventHub.EventStores;

namespace Revo.AzureEventHub.Modules
{
    /// <summary>
    /// Dependency injection module for Azure Event Hub event store.
    /// </summary>
    [AutoLoadModule(false)]
    public class AzureEventHubEventStoreModule : DependencyInjectionModule
    {
        private readonly AzureEventHubEventStoreConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureEventHubEventStoreModule"/> class.
        /// </summary>
        /// <param name="configuration">The Azure Event Hub configuration.</param>
        public AzureEventHubEventStoreModule(AzureEventHubEventStoreConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Configures the services for this module.
        /// </summary>
        /// <param name="registry">The service registry to register services with.</param>
        /// <param name="context">The module configuration context.</param>
        public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
        {
            // Register configuration
            registry.RegisterSingleton<AzureEventHubEventStoreConfiguration>(provider => configuration);

            // Register event store
            registry.RegisterScoped<IEventStore, AzureEventHubEventStore>();
            registry.RegisterScoped<IAzureEventHubEventStore, AzureEventHubEventStore>();

            // Register external event store
            registry.RegisterScoped<IExternalEventStore, AzureEventHubExternalEventStore>();
        }
    }
}
