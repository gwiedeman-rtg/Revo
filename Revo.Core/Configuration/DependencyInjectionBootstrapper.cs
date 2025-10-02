using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Revo.DependencyInjection.Core;

namespace Revo.Core.Configuration
{
    /// <summary>
    /// Bootstrapper for dependency injection using the new DI abstraction.
    /// This is the new version of KernelBootstrapper that uses the abstracted dependency injection system.
    /// </summary>
    public class DependencyInjectionBootstrapper
    {
        private readonly IServiceContainer _container;
        private readonly IModuleLoader _moduleLoader;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly HashSet<Assembly> _loadedAssemblies = new HashSet<Assembly>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionBootstrapper"/> class.
        /// </summary>
        /// <param name="container">The service container.</param>
        /// <param name="moduleLoader">The module loader.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public DependencyInjectionBootstrapper(
            IServiceContainer container, 
            IModuleLoader moduleLoader, 
            IConfiguration configuration, 
            ILogger logger)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _moduleLoader = moduleLoader ?? throw new ArgumentNullException(nameof(moduleLoader));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the service container.
        /// </summary>
        public IServiceContainer Container => _container;

        /// <summary>
        /// Gets the module loader.
        /// </summary>
        public IModuleLoader ModuleLoader => _moduleLoader;

        /// <summary>
        /// Configures the dependency injection container.
        /// </summary>
        public void Configure()
        {
            _logger.LogInformation("Configuring dependency injection container...");
            
            // Load configuration-based modules
            var kernelSection = _configuration.GetSection("DependencyInjection");
            var kernelActions = kernelSection.GetSection("KernelActions").Get<List<Action<IModuleConfigurationContext>>>();
            
            if (kernelActions != null)
            {
                foreach (var action in kernelActions)
                {
                    var context = CreateModuleConfigurationContext();
                    action(context);
                }
            }
            
            _logger.LogInformation("Dependency injection container configured successfully.");
        }

        /// <summary>
        /// Loads all dependency modules from the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for modules.</param>
        public void LoadAssemblies(IReadOnlyCollection<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                if (_loadedAssemblies.Contains(assembly))
                {
                    continue;
                }

                _logger.LogInformation("Loading modules from assembly: {AssemblyName}", assembly.FullName);
                _moduleLoader.LoadModules(new[] { assembly });
                _loadedAssemblies.Add(assembly);
            }
        }

        /// <summary>
        /// Runs application configurers.
        /// </summary>
        public void RunAppConfigurers()
        {
            try
            {
                var initializer = _container.GetService<IApplicationConfigurerInitializer>();
                initializer.ConfigureAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to run application configurers.");
                throw;
            }
        }

        /// <summary>
        /// Runs application start listeners.
        /// </summary>
        public void RunAppStartListeners()
        {
            try
            {
                var initializer = _container.GetService<IApplicationLifecycleNotifier>();
                initializer.NotifyStarting();
                initializer.NotifyStarted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to run application start listeners.");
                throw;
            }
        }

        /// <summary>
        /// Runs application stop listeners.
        /// </summary>
        public void RunAppStopListeners()
        {
            try
            {
                var initializer = _container.GetService<IApplicationLifecycleNotifier>();
                initializer.NotifyStopping();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to run application stop listeners.");
                throw;
            }
        }

        /// <summary>
        /// Gets all loaded modules.
        /// </summary>
        /// <returns>All currently loaded modules.</returns>
        public IEnumerable<IDependencyModule> GetLoadedModules()
        {
            return _moduleLoader.GetLoadedModules();
        }

        private IModuleConfigurationContext CreateModuleConfigurationContext()
        {
            // This would need to be implemented by the specific DI container implementation
            // For now, we'll create a basic implementation
            return new BasicModuleConfigurationContext(_configuration, _container);
        }

        private class BasicModuleConfigurationContext : IModuleConfigurationContext
        {
            public BasicModuleConfigurationContext(IConfiguration configuration, IServiceContainer container)
            {
                Configuration = configuration;
                ServiceProvider = container;
            }

            public IConfiguration Configuration { get; }
            public IServiceProvider ServiceProvider { get; }

            public bool IsModuleEnabled(Type moduleType)
            {
                var autoLoadAttr = moduleType.GetCustomAttribute<AutoLoadModuleAttribute>();
                bool autoLoad = autoLoadAttr?.AutoLoad ?? true;

                var configKey = $"DependencyInjection:Modules:{moduleType.FullName}:AutoLoad";
                if (Configuration.GetValue<bool?>($"{configKey}") is bool configValue)
                {
                    autoLoad = configValue;
                }

                return autoLoad;
            }

            public bool IsModuleEnabled<TModule>() where TModule : class, IDependencyModule
            {
                return IsModuleEnabled(typeof(TModule));
            }

            public IConfigurationSection GetSection(string sectionName)
            {
                return Configuration.GetSection(sectionName);
            }

            public TSection GetSection<TSection>() where TSection : class, new()
            {
                var sectionName = typeof(TSection).Name;
                if (sectionName.EndsWith("ConfigurationSection"))
                {
                    sectionName = sectionName.Substring(0, sectionName.Length - "ConfigurationSection".Length);
                }
                else if (sectionName.EndsWith("Configuration"))
                {
                    sectionName = sectionName.Substring(0, sectionName.Length - "Configuration".Length);
                }

                var section = Configuration.GetSection(sectionName);
                var result = new TSection();
                section.Bind(result);
                return result;
            }
        }
    }
}
