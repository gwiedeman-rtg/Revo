using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Ninject;
using Ninject.Modules;
using Revo.Core.Core;
using Revo.Core.Lifecycle;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Ninject;

namespace Revo.Core.Configuration
{
    /// <summary>
    /// Unified bootstrapper that can work with both the old Ninject system and the new DI abstraction.
    /// This provides backward compatibility while allowing gradual migration to the new system.
    /// </summary>
    public class UnifiedBootstrapper
    {
        private readonly IRevoConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly HashSet<Assembly> _loadedAssemblies = new HashSet<Assembly>();
        private readonly bool _useNewDiSystem;
        
        // Old system components
        private readonly IKernel _kernel;
        private readonly KernelBootstrapper _kernelBootstrapper;
        
        // New system components
        private readonly IServiceContainer _container;
        private readonly Revo.DependencyInjection.Core.IModuleLoader _moduleLoader;
        private readonly DependencyInjectionBootstrapper _diBootstrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedBootstrapper"/> class using the old Ninject system.
        /// </summary>
        /// <param name="kernel">The Ninject kernel.</param>
        /// <param name="configuration">The Revo configuration.</param>
        /// <param name="logger">The logger.</param>
        public UnifiedBootstrapper(IKernel kernel, IRevoConfiguration configuration, ILogger logger)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _useNewDiSystem = false;
            
            _kernelBootstrapper = new KernelBootstrapper(kernel, configuration, logger);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedBootstrapper"/> class using the new DI system.
        /// </summary>
        /// <param name="container">The service container.</param>
        /// <param name="moduleLoader">The module loader.</param>
        /// <param name="configuration">The Revo configuration.</param>
        /// <param name="logger">The logger.</param>
        public UnifiedBootstrapper(IServiceContainer container, Revo.DependencyInjection.Core.IModuleLoader moduleLoader, IRevoConfiguration configuration, ILogger logger)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _moduleLoader = moduleLoader ?? throw new ArgumentNullException(nameof(moduleLoader));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _useNewDiSystem = true;
            
            _diBootstrapper = new DependencyInjectionBootstrapper(container, moduleLoader, (Microsoft.Extensions.Configuration.IConfiguration)configuration, logger);
        }

        /// <summary>
        /// Gets the service container (new system) or null if using old system.
        /// </summary>
        public IServiceContainer Container => _container;

        /// <summary>
        /// Gets the Ninject kernel (old system) or null if using new system.
        /// </summary>
        public IKernel Kernel => _kernel;

        /// <summary>
        /// Gets a value indicating whether the new DI system is being used.
        /// </summary>
        public bool UseNewDiSystem => _useNewDiSystem;

        /// <summary>
        /// Configures the dependency injection system.
        /// </summary>
        public void Configure()
        {
            if (_useNewDiSystem)
            {
                _diBootstrapper.Configure();
            }
            else
            {
                _kernelBootstrapper.Configure();
            }
        }

        /// <summary>
        /// Loads all modules from the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for modules.</param>
        public void LoadAssemblies(IReadOnlyCollection<Assembly> assemblies)
        {
            if (_useNewDiSystem)
            {
                _diBootstrapper.LoadAssemblies(assemblies);
            }
            else
            {
                _kernelBootstrapper.LoadAssemblies(assemblies);
            }
        }

        /// <summary>
        /// Loads modules from assemblies, supporting both old and new module types.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for modules.</param>
        public void LoadAssembliesUnified(IReadOnlyCollection<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                if (_loadedAssemblies.Contains(assembly))
                {
                    continue;
                }

                _logger.LogInformation("Loading modules from assembly: {AssemblyName}", assembly.FullName);

                if (_useNewDiSystem)
                {
                    // Load new DI modules
                    _moduleLoader.LoadModules(new[] { assembly });
                    
                    // Also load old Ninject modules for backward compatibility
                    LoadLegacyNinjectModules(assembly);
                }
                else
                {
                    // Load old Ninject modules
                    _kernelBootstrapper.LoadAssemblies(new[] { assembly });
                }

                _loadedAssemblies.Add(assembly);
            }
        }

        /// <summary>
        /// Runs application configurers.
        /// </summary>
        public void RunAppConfigurers()
        {
            if (_useNewDiSystem)
            {
                _diBootstrapper.RunAppConfigurers();
            }
            else
            {
                _kernelBootstrapper.RunAppConfigurers();
            }
        }

        /// <summary>
        /// Runs application start listeners.
        /// </summary>
        public void RunAppStartListeners()
        {
            if (_useNewDiSystem)
            {
                _diBootstrapper.RunAppStartListeners();
            }
            else
            {
                _kernelBootstrapper.RunAppStartListeners();
            }
        }

        /// <summary>
        /// Runs application stop listeners.
        /// </summary>
        public void RunAppStopListeners()
        {
            if (_useNewDiSystem)
            {
                _diBootstrapper.RunAppStopListeners();
            }
            else
            {
                _kernelBootstrapper.RunAppStopListeners();
            }
        }

        /// <summary>
        /// Gets all loaded modules.
        /// </summary>
        /// <returns>All currently loaded modules.</returns>
        public IEnumerable<object> GetLoadedModules()
        {
            if (_useNewDiSystem)
            {
                return _diBootstrapper.GetLoadedModules().Cast<object>();
            }
            else
            {
                // For old system, we can't easily get loaded modules from KernelBootstrapper
                return Enumerable.Empty<object>();
            }
        }

        /// <summary>
        /// Gets a service from the container.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>The service instance.</returns>
        public T GetService<T>()
        {
            if (_useNewDiSystem)
            {
                return _container.GetService<T>();
            }
            else
            {
                return _kernel.Get<T>();
            }
        }

        /// <summary>
        /// Gets a service from the container.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The service instance.</returns>
        public object GetService(Type serviceType)
        {
            if (_useNewDiSystem)
            {
                return _container.GetService(serviceType);
            }
            else
            {
                return _kernel.Get(serviceType);
            }
        }

        private void LoadLegacyNinjectModules(Assembly assembly)
        {
            if (_container is NinjectServiceContainer ninjectContainer)
            {
                var modules = GetNinjectModules(assembly).Where(x => !ninjectContainer.Kernel.HasModule(x.Name)).ToArray();
                if (modules.Length > 0)
                {
                    _logger.LogInformation("Loading {ModuleCount} legacy Ninject modules from assembly {AssemblyName}: {ModuleNames}",
                        modules.Length, assembly.FullName, string.Join(",", modules.Select(x => x.Name)));
                    ninjectContainer.Kernel.Load(modules);
                }
            }
        }

        private INinjectModule[] GetNinjectModules(Assembly assembly)
        {
            return assembly.IsDynamic
                ? new INinjectModule[0]
                : assembly.ExportedTypes.Where(IsLoadableNinjectModule)
                    .Select(type => Activator.CreateInstance(type) as INinjectModule)
                    .ToArray();
        }

        private bool IsLoadableNinjectModule(Type type)
        {
            if (!typeof(INinjectModule).IsAssignableFrom(type)
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
            var autoLoadAttr = type.GetCustomAttribute<Revo.Core.Core.AutoLoadModuleAttribute>();
            bool autoLoad = autoLoadAttr?.AutoLoad ?? true;

            var kernelSection = _configuration.GetSection<KernelConfigurationSection>();
            if (kernelSection.LoadedModuleOverrides.TryGetValue(type, out bool autoLoadOverride))
            {
                autoLoad = autoLoadOverride;
            }

            return autoLoad;
        }
    }
}
