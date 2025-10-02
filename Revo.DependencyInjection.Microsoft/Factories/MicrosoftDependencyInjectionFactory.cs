using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Revo.DependencyInjection.Core;

namespace Revo.DependencyInjection.Microsoft
{
    /// <summary>
    /// Factory for creating Microsoft DI components.
    /// </summary>
    public static class MicrosoftDependencyInjectionFactory
    {
        /// <summary>
        /// Creates a new service collection.
        /// </summary>
        /// <returns>A new service collection.</returns>
        public static IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        /// <summary>
        /// Creates a service container from a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>A service container.</returns>
        public static IServiceContainer CreateServiceContainer(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var serviceProvider = services.BuildServiceProvider();
            return new MicrosoftServiceContainer(serviceProvider);
        }

        /// <summary>
        /// Creates a service container from a service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>A service container.</returns>
        public static IServiceContainer CreateServiceContainer(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            return new MicrosoftServiceContainer(serviceProvider);
        }

        /// <summary>
        /// Creates a service registry from a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>A service registry.</returns>
        public static IServiceRegistry CreateServiceRegistry(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return new MicrosoftServiceRegistry(services);
        }

        /// <summary>
        /// Creates a module loader.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A module loader.</returns>
        public static IModuleLoader CreateModuleLoader(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            return new MicrosoftModuleLoader(services, configuration, logger);
        }

        /// <summary>
        /// Creates a complete DI setup with service collection, container, and module loader.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configureServices">Optional action to configure services.</param>
        /// <returns>A tuple containing the service collection, container, and module loader.</returns>
        public static (IServiceCollection Services, IServiceContainer Container, IModuleLoader ModuleLoader) CreateCompleteSetup(
            IConfiguration configuration, 
            ILogger logger, 
            Action<IServiceCollection> configureServices = null)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            var services = CreateServiceCollection();
            configureServices?.Invoke(services);
            
            var container = CreateServiceContainer(services);
            var moduleLoader = CreateModuleLoader(services, configuration, logger);

            return (services, container, moduleLoader);
        }
    }
}

