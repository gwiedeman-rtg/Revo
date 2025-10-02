using System;
using Microsoft.Extensions.Configuration;

namespace Revo.DependencyInjection.Core
{
    /// <summary>
    /// Provides context information for module configuration.
    /// </summary>
    public interface IModuleConfigurationContext
    {
        /// <summary>
        /// Gets the configuration for the application.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the service provider for resolving dependencies during module configuration.
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Checks if a module type is enabled for loading.
        /// </summary>
        /// <param name="moduleType">The module type to check.</param>
        /// <returns>True if the module is enabled, false otherwise.</returns>
        bool IsModuleEnabled(Type moduleType);

        /// <summary>
        /// Checks if a module type is enabled for loading.
        /// </summary>
        /// <typeparam name="TModule">The module type to check.</typeparam>
        /// <returns>True if the module is enabled, false otherwise.</returns>
        bool IsModuleEnabled<TModule>() where TModule : class, IDependencyModule;

        /// <summary>
        /// Gets a configuration section by name.
        /// </summary>
        /// <param name="sectionName">The name of the configuration section.</param>
        /// <returns>The configuration section.</returns>
        IConfigurationSection GetSection(string sectionName);

        /// <summary>
        /// Gets a configuration section by type.
        /// </summary>
        /// <typeparam name="TSection">The type of the configuration section.</typeparam>
        /// <returns>The configuration section.</returns>
        TSection GetSection<TSection>() where TSection : class, new();
    }
}
