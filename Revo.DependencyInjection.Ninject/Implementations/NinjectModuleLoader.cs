using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ninject;
using Revo.DependencyInjection.Core;

namespace Revo.DependencyInjection.Ninject
{
    /// <summary>
    /// Ninject implementation of the module loader.
    /// </summary>
    public class NinjectModuleLoader : IModuleLoader
    {
        private readonly IKernel _kernel;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly List<IDependencyModule> _loadedModules = new List<IDependencyModule>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectModuleLoader"/> class.
        /// </summary>
        /// <param name="kernel">The Ninject kernel.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public NinjectModuleLoader(IKernel kernel, IConfiguration configuration, ILogger logger)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the underlying Ninject kernel.
        /// </summary>
        public IKernel Kernel => _kernel;

        /// <summary>
        /// Loads all dependency modules from the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for modules.</param>
        public void LoadModules(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                LoadModulesFromAssembly(assembly);
            }
        }

        /// <summary>
        /// Loads a specific module type.
        /// </summary>
        /// <typeparam name="TModule">The module type to load.</typeparam>
        public void LoadModule<TModule>() where TModule : class, IDependencyModule, new()
        {
            var module = new TModule();
            LoadModule(module);
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

            if (IsModuleLoaded(module.Name))
            {
                _logger.LogDebug("Module '{ModuleName}' is already loaded, skipping.", module.Name);
                return;
            }

            try
            {
                _logger.LogInformation("Loading module '{ModuleName}'.", module.Name);
                
                var context = new NinjectModuleConfigurationContext(_kernel, _configuration);
                var registry = new NinjectServiceRegistry(_kernel);
                
                module.Configure(registry, context);
                _loadedModules.Add(module);
                
                _logger.LogInformation("Successfully loaded module '{ModuleName}'.", module.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load module '{ModuleName}'.", module.Name);
                throw;
            }
        }

        /// <summary>
        /// Loads a specific module type with constructor parameters.
        /// </summary>
        /// <typeparam name="TModule">The module type to load.</typeparam>
        /// <param name="constructorArgs">Constructor arguments for the module.</param>
        public void LoadModule<TModule>(params object[] constructorArgs) where TModule : class, IDependencyModule
        {
            var module = (TModule)Activator.CreateInstance(typeof(TModule), constructorArgs);
            LoadModule(module);
        }

        /// <summary>
        /// Gets all loaded modules.
        /// </summary>
        /// <returns>All currently loaded modules.</returns>
        public IEnumerable<IDependencyModule> GetLoadedModules()
        {
            return _loadedModules.ToList();
        }

        /// <summary>
        /// Checks if a module is loaded.
        /// </summary>
        /// <param name="moduleName">The name of the module to check.</param>
        /// <returns>True if the module is loaded, false otherwise.</returns>
        public bool IsModuleLoaded(string moduleName)
        {
            return _loadedModules.Any(m => m.Name == moduleName);
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

        private void LoadModulesFromAssembly(Assembly assembly)
        {
            if (assembly.IsDynamic)
            {
                return;
            }

            var moduleTypes = assembly.ExportedTypes
                .Where(IsLoadableModule)
                .ToList();

            if (moduleTypes.Count == 0)
            {
                return;
            }

            _logger.LogInformation("Found {ModuleCount} modules in assembly '{AssemblyName}': {ModuleNames}",
                moduleTypes.Count, assembly.FullName, string.Join(", ", moduleTypes.Select(t => t.Name)));

            foreach (var moduleType in moduleTypes)
            {
                try
                {
                    var module = (IDependencyModule)Activator.CreateInstance(moduleType);
                    LoadModule(module);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load module '{ModuleType}' from assembly '{AssemblyName}'.",
                        moduleType.FullName, assembly.FullName);
                    throw;
                }
            }
        }

        private bool IsLoadableModule(Type type)
        {
            if (!typeof(IDependencyModule).IsAssignableFrom(type)
                || type.IsAbstract
                || type.IsInterface
                || type.GetConstructor(Type.EmptyTypes) == null)
            {
                return false;
            }

            return IsModuleEnabled(type);
        }

        private bool IsModuleEnabled(Type type)
        {
            var autoLoadAttr = type.GetCustomAttribute<AutoLoadModuleAttribute>();
            bool autoLoad = autoLoadAttr?.AutoLoad ?? true;

            // Check configuration overrides
            var configKey = $"DependencyInjection:Modules:{type.FullName}:AutoLoad";
            var configValueStr = _configuration[configKey];
            if (bool.TryParse(configValueStr, out bool configValue))
            {
                autoLoad = configValue;
            }

            return autoLoad;
        }
    }
}
