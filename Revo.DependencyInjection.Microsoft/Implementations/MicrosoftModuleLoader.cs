using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Revo.DependencyInjection.Core;
using Revo.Core.Core;
using MSConfig = Microsoft.Extensions.Configuration;

namespace Revo.DependencyInjection.Microsoft
{
    /// <summary>
    /// Microsoft DI implementation of the module loader.
    /// </summary>
    public class MicrosoftModuleLoader : IModuleLoader
    {
        private readonly IServiceCollection _services;
        private readonly MSConfig.IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly List<IDependencyModule> _loadedModules;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftModuleLoader"/> class.
        /// </summary>
        /// <param name="services">The Microsoft service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public MicrosoftModuleLoader(IServiceCollection services, MSConfig.IConfiguration configuration, ILogger logger)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _loadedModules = new List<IDependencyModule>();
        }

        /// <summary>
        /// Gets the loaded modules.
        /// </summary>
        public IReadOnlyList<IDependencyModule> LoadedModules => _loadedModules.AsReadOnly();

        /// <summary>
        /// Loads a module by type.
        /// </summary>
        /// <typeparam name="TModule">The module type.</typeparam>
        public void LoadModule<TModule>() where TModule : class, IDependencyModule, new()
        {
            LoadModule(typeof(TModule));
        }

        /// <summary>
        /// Loads a module by type.
        /// </summary>
        /// <param name="moduleType">The module type.</param>
        public void LoadModule(Type moduleType)
        {
            if (moduleType == null)
            {
                throw new ArgumentNullException(nameof(moduleType));
            }

            if (!typeof(IDependencyModule).IsAssignableFrom(moduleType))
            {
                throw new ArgumentException($"Type {moduleType.Name} does not implement IDependencyModule", nameof(moduleType));
            }

            // Check if module is already loaded
            if (_loadedModules.Any(m => m.GetType() == moduleType))
            {
                _logger.LogDebug("Module {ModuleType} is already loaded, skipping", moduleType.Name);
                return;
            }

            // Check if module is enabled
            var context = new MicrosoftModuleConfigurationContext(_configuration, _logger);
            if (!IsModuleEnabled(moduleType, context))
            {
                _logger.LogDebug("Module {ModuleType} is disabled, skipping", moduleType.Name);
                return;
            }

            // Create and configure the module
            var module = CreateModule(moduleType);
            ConfigureModule(module, context);
            
            _loadedModules.Add(module);
            _logger.LogInformation("Loaded module {ModuleType}", moduleType.Name);
        }

        /// <summary>
        /// Loads modules from an assembly.
        /// </summary>
        /// <param name="assembly">The assembly to load modules from.</param>
        public void LoadModulesFromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var moduleTypes = assembly.GetTypes()
                .Where(t => typeof(IDependencyModule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                .ToList();

            foreach (var moduleType in moduleTypes)
            {
                try
                {
                    LoadModule(moduleType);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load module {ModuleType} from assembly {Assembly}", 
                        moduleType.Name, assembly.GetName().Name);
                    throw;
                }
            }
        }

        /// <summary>
        /// Loads modules from multiple assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to load modules from.</param>
        public void LoadModulesFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            foreach (var assembly in assemblies)
            {
                LoadModulesFromAssembly(assembly);
            }
        }

        /// <summary>
        /// Loads all dependency modules from the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for modules.</param>
        public void LoadModules(IEnumerable<Assembly> assemblies)
        {
            LoadModulesFromAssemblies(assemblies);
        }

        /// <summary>
        /// Loads a specific module instance.
        /// </summary>
        /// <param name="module">The module instance to load.</param>
        public void LoadModule(IDependencyModule module)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            // Check if module is already loaded
            if (_loadedModules.Any(m => m.GetType() == module.GetType()))
            {
                _logger.LogDebug("Module {ModuleType} is already loaded, skipping", module.GetType().Name);
                return;
            }

            // Check if module is enabled
            var context = new MicrosoftModuleConfigurationContext(_configuration, _logger);
            if (!IsModuleEnabled(module.GetType(), context))
            {
                _logger.LogDebug("Module {ModuleType} is disabled, skipping", module.GetType().Name);
                return;
            }

            // Configure the module
            ConfigureModule(module, context);
            
            _loadedModules.Add(module);
            _logger.LogInformation("Loaded module {ModuleType}", module.GetType().Name);
        }

        /// <summary>
        /// Loads a specific module type with constructor parameters.
        /// </summary>
        /// <typeparam name="TModule">The module type to load.</typeparam>
        /// <param name="constructorArgs">Constructor arguments for the module.</param>
        public void LoadModule<TModule>(params object[] constructorArgs) where TModule : class, IDependencyModule
        {
            if (typeof(TModule) == null)
            {
                throw new ArgumentNullException(nameof(TModule));
            }

            // Check if module is already loaded
            if (_loadedModules.Any(m => m.GetType() == typeof(TModule)))
            {
                _logger.LogDebug("Module {ModuleType} is already loaded, skipping", typeof(TModule).Name);
                return;
            }

            // Check if module is enabled
            var context = new MicrosoftModuleConfigurationContext(_configuration, _logger);
            if (!IsModuleEnabled(typeof(TModule), context))
            {
                _logger.LogDebug("Module {ModuleType} is disabled, skipping", typeof(TModule).Name);
                return;
            }

            // Create and configure the module
            var module = CreateModuleWithArgs<TModule>(constructorArgs);
            ConfigureModule(module, context);
            
            _loadedModules.Add(module);
            _logger.LogInformation("Loaded module {ModuleType}", typeof(TModule).Name);
        }

        /// <summary>
        /// Gets all loaded modules.
        /// </summary>
        /// <returns>All currently loaded modules.</returns>
        public IEnumerable<IDependencyModule> GetLoadedModules()
        {
            return _loadedModules.AsReadOnly();
        }

        /// <summary>
        /// Checks if a module is loaded.
        /// </summary>
        /// <param name="moduleName">The name of the module to check.</param>
        /// <returns>True if the module is loaded, false otherwise.</returns>
        public bool IsModuleLoaded(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
            {
                return false;
            }

            return _loadedModules.Any(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if a module is loaded.
        /// </summary>
        /// <typeparam name="TModule">The module type to check.</typeparam>
        /// <returns>True if the module is loaded, false otherwise.</returns>
        public bool IsModuleLoaded<TModule>() where TModule : class, IDependencyModule
        {
            return _loadedModules.Any(m => m is TModule);
        }

        /// <summary>
        /// Creates a module instance.
        /// </summary>
        /// <param name="moduleType">The module type.</param>
        /// <returns>The created module instance.</returns>
        private IDependencyModule CreateModule(Type moduleType)
        {
            try
            {
                return (IDependencyModule)Activator.CreateInstance(moduleType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create instance of module {ModuleType}", moduleType.Name);
                throw new InvalidOperationException($"Failed to create instance of module {moduleType.Name}", ex);
            }
        }

        /// <summary>
        /// Creates a module instance with constructor arguments.
        /// </summary>
        /// <typeparam name="TModule">The module type.</typeparam>
        /// <param name="constructorArgs">Constructor arguments.</param>
        /// <returns>The created module instance.</returns>
        private TModule CreateModuleWithArgs<TModule>(object[] constructorArgs) where TModule : class, IDependencyModule
        {
            try
            {
                return (TModule)Activator.CreateInstance(typeof(TModule), constructorArgs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create instance of module {ModuleType}", typeof(TModule).Name);
                throw new InvalidOperationException($"Failed to create instance of module {typeof(TModule).Name}", ex);
            }
        }

        /// <summary>
        /// Configures a module.
        /// </summary>
        /// <param name="module">The module to configure.</param>
        /// <param name="context">The configuration context.</param>
        private void ConfigureModule(IDependencyModule module, IModuleConfigurationContext context)
        {
            try
            {
                if (module is MicrosoftDependencyModule microsoftModule)
                {
                    // Use Microsoft-specific configuration
                    microsoftModule.Configure(_services, _configuration, _logger);
                }
                else
                {
                    // Use generic configuration
                    var registry = new MicrosoftServiceRegistry(_services);
                    module.Configure(registry, context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to configure module {ModuleType}", module.GetType().Name);
                throw new InvalidOperationException($"Failed to configure module {module.GetType().Name}", ex);
            }
        }

        /// <summary>
        /// Checks if a module is enabled.
        /// </summary>
        /// <param name="moduleType">The module type.</param>
        /// <param name="context">The configuration context.</param>
        /// <returns>True if the module is enabled.</returns>
        private bool IsModuleEnabled(Type moduleType, IModuleConfigurationContext context)
        {
            var autoLoadAttribute = moduleType.GetCustomAttribute<Revo.Core.Core.AutoLoadModuleAttribute>();
            
            if (autoLoadAttribute == null)
            {
                return true; // No attribute means enabled by default
            }

            if (!autoLoadAttribute.AutoLoad)
            {
                return false; // Explicitly disabled
            }

            // Check configuration for module-specific settings
            var moduleName = moduleType.Name;
            var configKey = $"Modules:{moduleName}:Enabled";
            return _configuration.GetValue(configKey, true);
        }
    }
}
