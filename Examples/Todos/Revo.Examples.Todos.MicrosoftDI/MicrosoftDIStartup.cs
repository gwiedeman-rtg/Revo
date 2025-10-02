using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Revo.AspNetCore.Core;
using Revo.AspNetCore.Ninject;
using Revo.Core.Configuration;
using Revo.Core.Core;
using Revo.Core.Logging;
using Revo.Core.Types;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Microsoft;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Revo.Examples.Todos.MicrosoftDI
{
    /// <summary>
    /// Revo startup class using Microsoft DI implementation.
    /// This provides a modern way to bootstrap Revo applications with ASP.NET Core using Microsoft DI.
    /// </summary>
    public abstract class MicrosoftDIStartup
    {
        private static readonly AsyncLocal<Scope> scopeProvider = new();
        public static object RequestScope(object context) => scopeProvider.Value;
        
        private DependencyInjectionBootstrapper _bootstrapper;
        private ITypeExplorer _typeExplorer;
        private IServiceContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftDIStartup"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public MicrosoftDIStartup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Gets the service container.
        /// </summary>
        public IServiceContainer Container => _container;

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            CreateContainer();
            var revoConfiguration = CreateRevoConfiguration();

            // Configure ASP.NET Core services
            services.AddMvcCore();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(sp => _container);

            // Note: Microsoft DI handles request scoping automatically
            // No need for custom activation as Microsoft DI handles this

            // Create bootstrapper
            _bootstrapper = new DependencyInjectionBootstrapper(
                _container, 
                CreateModuleLoader(), 
                Configuration, 
                LoggerFactory.CreateLogger<DependencyInjectionBootstrapper>());

            _typeExplorer = new TypeExplorer(LoggerFactory.CreateLogger<TypeExplorer>());

            // Configure the container
            _bootstrapper.Configure();

            // Load assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic).ToList();

            _bootstrapper.LoadAssemblies(assemblies);
            
            var aspNetCoreConfigurers = _container.GetServices<IAspNetCoreStartupConfigurer>();
            foreach (var aspNetCoreConfigurer in aspNetCoreConfigurers)
            {
                aspNetCoreConfigurer.ConfigureServices(services);
            }
        }
        
        /// <summary>
        /// Configures the application pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web host environment.</param>
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            RegisterAspNetCoreService(app);
            _bootstrapper.RunAppConfigurers();

            var aspNetCoreConfigurers = _container.GetServices<IAspNetCoreStartupConfigurer>();
            foreach (var aspNetCoreConfigurer in aspNetCoreConfigurers)
            {
                aspNetCoreConfigurer.Configure(app, env);
            }
            
            _bootstrapper.RunAppStartListeners();
        }

        /// <summary>
        /// Creates the service container.
        /// </summary>
        private void CreateContainer()
        {
            var services = new ServiceCollection();
            _container = MicrosoftDependencyInjectionFactory.CreateServiceContainer(services);
        }

        /// <summary>
        /// Creates the module loader.
        /// </summary>
        /// <returns>The module loader.</returns>
        protected virtual IModuleLoader CreateModuleLoader()
        {
            var services = new ServiceCollection();
            return MicrosoftDependencyInjectionFactory.CreateModuleLoader(
                services,
                Configuration, 
                LoggerFactory.CreateLogger<IModuleLoader>());
        }

        /// <summary>
        /// Creates the Revo configuration.
        /// </summary>
        /// <returns>The Revo configuration.</returns>
        protected virtual IRevoConfiguration CreateRevoConfiguration()
        {
            var configuration = new RevoConfiguration();
            
            // Configure core services
            configuration.ConfigureCoreWithNewDi();
            
            // Configure additional services
            ConfigureRevoConfiguration(configuration);
            
            return configuration;
        }

        /// <summary>
        /// Configures additional Revo configuration.
        /// Override this method to add custom configuration.
        /// </summary>
        /// <param name="configuration">The Revo configuration.</param>
        protected virtual void ConfigureRevoConfiguration(IRevoConfiguration configuration)
        {
            // Override in derived classes to add custom configuration
        }

        /// <summary>
        /// Configures additional ASP.NET Core services.
        /// Override this method to add custom services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        protected virtual void ConfigureAdditionalServices(IServiceCollection services)
        {
            // Override in derived classes to add custom services
        }

        private void RegisterAspNetCoreService(IApplicationBuilder app)
        {
            // Microsoft DI handles controller and hub registration automatically
            // No need for manual registration
        }
        
        private sealed class Scope : IDisposable
        {
            public void Dispose()
            {
                // Cleanup if needed
            }
        }
    }
}
