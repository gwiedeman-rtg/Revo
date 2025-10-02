using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Ninject;

namespace Revo.Core.Configuration
{
    /// <summary>
    /// Startup class for Revo applications using the new dependency injection abstraction.
    /// This provides a clean way to bootstrap Revo applications with any DI container.
    /// </summary>
    public class RevoDependencyInjectionStartup
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private IServiceContainer _container;
        private IModuleLoader _moduleLoader;
        private DependencyInjectionBootstrapper _bootstrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RevoDependencyInjectionStartup"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="logger">The logger.</param>
        public RevoDependencyInjectionStartup(IConfiguration configuration, ILogger logger)
        {
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
        /// Configures the dependency injection container using Ninject.
        /// </summary>
        /// <returns>The configured service container.</returns>
        public IServiceContainer ConfigureWithNinject()
        {
            _logger.LogInformation("Configuring Revo with Ninject dependency injection...");

            // Create Ninject kernel
            var kernel = NinjectDependencyInjectionFactory.CreateKernel(kernel =>
            {
                // Configure Ninject-specific settings
                kernel.Components.Add<Ninject.Planning.Bindings.Resolvers.IBindingResolver, ContravariantBindingResolver>();
            });

            // Create service container and module loader
            _container = NinjectDependencyInjectionFactory.CreateServiceContainer(kernel);
            _moduleLoader = NinjectDependencyInjectionFactory.CreateModuleLoader(kernel, _configuration, _logger);

            // Create bootstrapper
            _bootstrapper = new DependencyInjectionBootstrapper(_container, _moduleLoader, _configuration, _logger);

            // Configure the container
            _bootstrapper.Configure();

            _logger.LogInformation("Revo configured with Ninject successfully.");
            return _container;
        }

        /// <summary>
        /// Configures the dependency injection container using a custom implementation.
        /// </summary>
        /// <param name="container">The service container.</param>
        /// <param name="moduleLoader">The module loader.</param>
        /// <returns>The configured service container.</returns>
        public IServiceContainer ConfigureWithCustom(IServiceContainer container, IModuleLoader moduleLoader)
        {
            _logger.LogInformation("Configuring Revo with custom dependency injection...");

            _container = container ?? throw new ArgumentNullException(nameof(container));
            _moduleLoader = moduleLoader ?? throw new ArgumentNullException(nameof(moduleLoader));

            // Create bootstrapper
            _bootstrapper = new DependencyInjectionBootstrapper(_container, _moduleLoader, _configuration, _logger);

            // Configure the container
            _bootstrapper.Configure();

            _logger.LogInformation("Revo configured with custom dependency injection successfully.");
            return _container;
        }

        /// <summary>
        /// Loads all modules from the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for modules.</param>
        public void LoadModules(IEnumerable<System.Reflection.Assembly> assemblies)
        {
            if (_bootstrapper == null)
            {
                throw new InvalidOperationException("Must call ConfigureWithNinject() or ConfigureWithCustom() first.");
            }

            _bootstrapper.LoadAssemblies(assemblies.ToList());
        }

        /// <summary>
        /// Loads all modules from the current application domain.
        /// </summary>
        public void LoadModulesFromCurrentDomain()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.GetName().Name.StartsWith("System."))
                .Where(a => !a.IsDynamic)
                .ToList();

            LoadModules(assemblies);
        }

        /// <summary>
        /// Runs application initialization.
        /// </summary>
        public void RunApplicationInitialization()
        {
            if (_bootstrapper == null)
            {
                throw new InvalidOperationException("Must call ConfigureWithNinject() or ConfigureWithCustom() first.");
            }

            _logger.LogInformation("Running application initialization...");

            try
            {
                _bootstrapper.RunAppConfigurers();
                _bootstrapper.RunAppStartListeners();
                
                _logger.LogInformation("Application initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Application initialization failed.");
                throw;
            }
        }

        /// <summary>
        /// Runs application shutdown.
        /// </summary>
        public void RunApplicationShutdown()
        {
            if (_bootstrapper == null)
            {
                throw new InvalidOperationException("Must call ConfigureWithNinject() or ConfigureWithCustom() first.");
            }

            _logger.LogInformation("Running application shutdown...");

            try
            {
                _bootstrapper.RunAppStopListeners();
                
                _logger.LogInformation("Application shutdown completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Application shutdown failed.");
                throw;
            }
        }

        /// <summary>
        /// Gets all loaded modules.
        /// </summary>
        /// <returns>All currently loaded modules.</returns>
        public IEnumerable<IDependencyModule> GetLoadedModules()
        {
            return _bootstrapper?.GetLoadedModules() ?? Enumerable.Empty<IDependencyModule>();
        }
    }
}
