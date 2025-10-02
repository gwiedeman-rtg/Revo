using System;

namespace Revo.DependencyInjection.Core
{
    /// <summary>
    /// Represents a dependency injection module that can register services.
    /// This is the base interface for all dependency modules in the Revo framework.
    /// </summary>
    public interface IDependencyModule
    {
        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this module should be automatically loaded.
        /// </summary>
        bool AutoLoad { get; }

        /// <summary>
        /// Configures the services for this module.
        /// </summary>
        /// <param name="registry">The service registry to register services with.</param>
        /// <param name="context">The module configuration context.</param>
        void Configure(IServiceRegistry registry, IModuleConfigurationContext context);
    }
}
