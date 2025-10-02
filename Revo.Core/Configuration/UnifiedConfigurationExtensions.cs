using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ninject;
using Revo.Core.Commands;
using Revo.Core.Core;
using Revo.Core.Security;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Ninject;

namespace Revo.Core.Configuration
{
    /// <summary>
    /// Configuration extensions that work with both the old Ninject system and the new DI abstraction.
    /// </summary>
    public static class UnifiedConfigurationExtensions
    {
        /// <summary>
        /// Configures the core services using the new DI system.
        /// </summary>
        /// <param name="configuration">The Revo configuration.</param>
        /// <param name="advancedAction">Advanced configuration action.</param>
        /// <returns>The configuration for method chaining.</returns>
        public static IRevoConfiguration ConfigureCoreWithNewDi(this IRevoConfiguration configuration,
            Action<CoreConfigurationSection> advancedAction = null)
        {
            var section = configuration.GetSection<CoreConfigurationSection>();
            advancedAction?.Invoke(section);

            // Register core modules using the new DI system
            configuration.ConfigureDependencyInjection(registry =>
            {
                // Load core modules
                var coreModule = new CoreDependencyModule(section);
                var commandsModule = new CommandsDependencyModule(section.Commands);
                
                // Note: Module configuration would be handled by the module loader
                // This is a simplified implementation for demonstration
            });

            return configuration;
        }

        /// <summary>
        /// Configures the core services using the old Ninject system.
        /// </summary>
        /// <param name="configuration">The Revo configuration.</param>
        /// <param name="advancedAction">Advanced configuration action.</param>
        /// <returns>The configuration for method chaining.</returns>
        public static IRevoConfiguration ConfigureCoreWithNinject(this IRevoConfiguration configuration,
            Action<CoreConfigurationSection> advancedAction = null)
        {
            var section = configuration.GetSection<CoreConfigurationSection>();
            advancedAction?.Invoke(section);

            configuration.ConfigureKernel(c =>
            {
                c.LoadModule(new CoreModule(section));
                c.LoadModule(new CommandsModule(section.Commands));

                if (section.Security.UseNullSecurityModule)
                {
                    c.LoadModule(new NullCoreSecurityModule());
                }
            });

            return configuration;
        }

        /// <summary>
        /// Configures dependency injection with a custom service registry.
        /// </summary>
        /// <param name="configuration">The Revo configuration.</param>
        /// <param name="configureAction">The configuration action.</param>
        /// <returns>The configuration for method chaining.</returns>
        public static IRevoConfiguration ConfigureDependencyInjection(this IRevoConfiguration configuration,
            Action<IServiceRegistry> configureAction)
        {
            // This would need to be implemented by the specific DI container
            // For now, we'll add it to the configuration for later use
            var section = configuration.GetSection<DependencyInjectionConfigurationSection>();
            section.AddConfigurationAction(configureAction);
            return configuration;
        }

        /// <summary>
        /// Creates a unified bootstrapper using the new DI system.
        /// </summary>
        /// <param name="configuration">The Revo configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A unified bootstrapper.</returns>
        public static UnifiedBootstrapper CreateUnifiedBootstrapperWithNewDi(this IRevoConfiguration configuration, ILogger logger)
        {
            // Create Ninject kernel for backward compatibility
            var kernel = NinjectDependencyInjectionFactory.CreateKernel();
            
            // Create new DI system components
            var container = NinjectDependencyInjectionFactory.CreateServiceContainer(kernel);
            var moduleLoader = NinjectDependencyInjectionFactory.CreateModuleLoader(kernel, (Microsoft.Extensions.Configuration.IConfiguration)configuration, logger);
            
            return new UnifiedBootstrapper(container, moduleLoader, configuration, logger);
        }

        /// <summary>
        /// Creates a unified bootstrapper using the old Ninject system.
        /// </summary>
        /// <param name="configuration">The Revo configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A unified bootstrapper.</returns>
        public static UnifiedBootstrapper CreateUnifiedBootstrapperWithNinject(this IRevoConfiguration configuration, ILogger logger)
        {
            var kernel = new StandardKernel();
            return new UnifiedBootstrapper(kernel, configuration, logger);
        }

        /// <summary>
        /// Creates a Revo startup using the new DI system.
        /// </summary>
        /// <param name="configuration">The Revo configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A Revo startup instance.</returns>
        public static RevoDependencyInjectionStartup CreateRevoStartup(this IRevoConfiguration configuration, ILogger logger)
        {
            return new RevoDependencyInjectionStartup((Microsoft.Extensions.Configuration.IConfiguration)configuration, logger);
        }
    }

    /// <summary>
    /// Basic module configuration context implementation.
    /// </summary>
    internal class BasicModuleConfigurationContext : IModuleConfigurationContext
    {
        public BasicModuleConfigurationContext(Microsoft.Extensions.Configuration.IConfiguration configuration, IServiceContainer container)
        {
            Configuration = configuration;
            ServiceProvider = (IServiceProvider)container;
        }

        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }
        public IServiceProvider ServiceProvider { get; }

        public bool IsModuleEnabled(Type moduleType)
        {
            return true; // Simplified implementation
        }

        public bool IsModuleEnabled<TModule>() where TModule : class, IDependencyModule
        {
            return true; // Simplified implementation
        }

        public Microsoft.Extensions.Configuration.IConfigurationSection GetSection(string key)
        {
            return Configuration.GetSection(key);
        }

        public TSection GetSection<TSection>() where TSection : class, new()
        {
            return new TSection(); // Simplified implementation
        }

        public TSection GetConfigurationSection<TSection>() where TSection : class, new()
        {
            return new TSection(); // Simplified implementation
        }
    }

    /// <summary>
    /// Configuration section for dependency injection settings.
    /// </summary>
    public class DependencyInjectionConfigurationSection : IRevoConfigurationSection
    {
        private readonly List<Action<IServiceRegistry>> _configurationActions = new List<Action<IServiceRegistry>>();

        /// <summary>
        /// Gets the configuration actions.
        /// </summary>
        public IReadOnlyCollection<Action<IServiceRegistry>> ConfigurationActions => _configurationActions;

        /// <summary>
        /// Adds a configuration action.
        /// </summary>
        /// <param name="action">The configuration action.</param>
        public void AddConfigurationAction(Action<IServiceRegistry> action)
        {
            _configurationActions.Add(action);
        }
    }
}
