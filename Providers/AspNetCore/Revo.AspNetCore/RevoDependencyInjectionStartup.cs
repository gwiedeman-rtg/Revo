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
using Revo.DependencyInjection.Ninject;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Revo.AspNetCore
{
    /// <summary>
    /// Revo startup class using the new dependency injection abstraction.
    /// This provides a modern way to bootstrap Revo applications with ASP.NET Core.
    /// </summary>
    public abstract class RevoDependencyInjectionStartup
    {
        private static readonly AsyncLocal<Scope> scopeProvider = new();
        public static object RequestScope(object context) => scopeProvider.Value;
        
        private DependencyInjectionBootstrapper _bootstrapper;
        private ITypeExplorer _typeExplorer;
        private IServiceContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="RevoDependencyInjectionStartup"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public RevoDependencyInjectionStartup(IConfiguration configuration, ILoggerFactory loggerFactory)
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

            // Add request scoping middleware
            services.AddRequestScopingMiddleware(() => scopeProvider.Value = new Scope());
            services.AddCustomControllerActivation();
            services.AddCustomHubActivation();
            services.AddCustomViewComponentActivation();

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
            var assemblies = _typeExplorer
                .GetAllReferencedAssemblies()
                .Where(a => !a.GetName().Name.StartsWith("System."))
                .Where(a => !a.IsDynamic)
                .ToList();

            _bootstrapper.LoadAssemblies(assemblies);

            // Configure additional services
            ConfigureAdditionalServices(services, revoConfiguration);
        }

        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web host environment.</param>
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure ASP.NET Core pipeline
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // endpoints.MapHubs(); // Commented out - requires SignalR package
            });

            // Run application initialization
            _bootstrapper.RunAppConfigurers();
            _bootstrapper.RunAppStartListeners();
        }

        /// <summary>
        /// Creates the service container.
        /// </summary>
        protected virtual void CreateContainer()
        {
            // Create Ninject kernel with extensions
            var kernel = NinjectDependencyInjectionFactory.CreateKernel(kernel =>
            {
                // Add Ninject extensions - commented out as modules are not available
                // kernel.Load(new FuncModule());
                // kernel.Load(new ContextPreservationModule());
            });

            _container = NinjectDependencyInjectionFactory.CreateServiceContainer(kernel);
        }

        /// <summary>
        /// Creates the module loader.
        /// </summary>
        /// <returns>The module loader.</returns>
        protected virtual IModuleLoader CreateModuleLoader()
        {
            if (_container is NinjectServiceContainer ninjectContainer)
            {
                return NinjectDependencyInjectionFactory.CreateModuleLoader(
                    ninjectContainer.Kernel, 
                    Configuration, 
                    LoggerFactory.CreateLogger<IModuleLoader>());
            }

            throw new NotSupportedException("Only Ninject service container is currently supported.");
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
        /// <param name="revoConfiguration">The Revo configuration.</param>
        protected virtual void ConfigureAdditionalServices(IServiceCollection services, IRevoConfiguration revoConfiguration)
        {
            // Configure ASP.NET Core specific services
            if (_container is NinjectServiceContainer ninjectContainer)
            {
                // Bind ASP.NET Core services to Ninject
                ninjectContainer.Kernel.Bind<IViewBufferScope>().ToMethod(ctx => 
                    services.BuildServiceProvider().GetRequiredService<IViewBufferScope>());
                
                ninjectContainer.Kernel.Bind(typeof(IHubContext<>), typeof(IHubContext<,>))
                    .ToMethod(ctx => services.BuildServiceProvider().GetRequiredService(ctx.Request.Service));

                ninjectContainer.Kernel.Bind<ILoggerFactory>().ToConstant(LoggerFactory);
                ninjectContainer.Kernel.Bind(typeof(ILogger<>)).ToMethod(context =>
                    LoggerFactory.CreateGenericLogger(context.GenericArguments[0]));

                ninjectContainer.Kernel.Bind<ILogger>().ToMethod(context => LoggerFactory.CreateLogger(
                    context.Request.Target?.Member.ReflectedType
                    ?? throw new InvalidOperationException(
                        "Non-generic ILogger can only be injected into other classes and can not be resolved on its own")));

                ninjectContainer.Kernel.Bind<IServiceProvider>().ToMethod(ctx => 
                    services.BuildServiceProvider());
                ninjectContainer.Kernel.Bind<IWebHostEnvironment>().ToMethod(ctx => 
                    services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>());
                ninjectContainer.Kernel.Bind<IHttpContextAccessor>()
                    .ToMethod(ctx => services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>())
                    .InTransientScope();
                ninjectContainer.Kernel.Bind<HttpContext>()
                    .ToMethod(ctx => services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>().HttpContext)
                    .InTransientScope();

                ninjectContainer.Kernel.Bind<IConfiguration>().ToConstant(Configuration);
            }
        }

        /// <summary>
        /// Disposes the startup and runs cleanup.
        /// </summary>
        public virtual void Dispose()
        {
            try
            {
                _bootstrapper?.RunAppStopListeners();
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLogger<RevoDependencyInjectionStartup>()
                    .LogError(ex, "Error during application shutdown");
            }
        }

        private sealed class Scope : IDisposable
        {
            public void Dispose()
            {
                // Scope disposal handled by Ninject
            }
        }
    }
}
