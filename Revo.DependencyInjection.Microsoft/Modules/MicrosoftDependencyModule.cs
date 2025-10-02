using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Revo.DependencyInjection.Core;

namespace Revo.DependencyInjection.Microsoft
{
    /// <summary>
    /// Base class for Microsoft DI dependency modules.
    /// This provides a convenient base class for modules that will be configured using the new DI abstraction
    /// but leverage Microsoft.Extensions.DependencyInjection internally.
    /// </summary>
    public abstract class MicrosoftDependencyModule : DependencyModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftDependencyModule"/> class.
        /// </summary>
        protected MicrosoftDependencyModule()
        {
        }

        /// <summary>
        /// Configures the services for this module using Microsoft DI.
        /// </summary>
        /// <param name="services">The Microsoft service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public virtual void Configure(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var registry = new MicrosoftServiceRegistry(services);
            var context = new MicrosoftModuleConfigurationContext(configuration, logger);
            Configure(registry, context);
        }

        /// <summary>
        /// Configures the services for this module.
        /// </summary>
        /// <param name="registry">The service registry to register services with.</param>
        /// <param name="context">The module configuration context.</param>
        public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
        {
            // Default implementation - derived classes should override Configure(IServiceCollection, IConfiguration, ILogger)
            // or this method to provide their specific configuration
        }
    }
}
