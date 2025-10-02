using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Revo.Core.Configuration;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Ninject;

namespace Revo.Examples.DependencyInjection
{
    /// <summary>
    /// Example application demonstrating the new Revo dependency injection system.
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Revo Dependency Injection Example");
            Console.WriteLine("==================================");
            Console.WriteLine();

            // Create configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                // .AddEnvironmentVariables() // Commented out - requires additional package
                .Build();

            // Create logger
            var loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Information));
            var logger = loggerFactory.CreateLogger<Program>();

            try
            {
                // Example 1: Using the new DI system directly
                await RunDirectDiExample(configuration, logger);

                Console.WriteLine();

                // Example 2: Using the Revo startup class
                await RunRevoStartupExample(configuration, logger);

                Console.WriteLine();

                // Example 3: Using the unified bootstrapper
                await RunUnifiedBootstrapperExample(configuration, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during execution");
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                loggerFactory.Dispose();
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Example 1: Using the new DI system directly
        /// </summary>
        static async Task RunDirectDiExample(IConfiguration configuration, ILogger logger)
        {
            logger.LogInformation("=== Example 1: Direct DI System Usage ===");

            // Create Ninject kernel
            var kernel = NinjectDependencyInjectionFactory.CreateKernel();

            // Create service container and module loader
            var container = NinjectDependencyInjectionFactory.CreateServiceContainer(kernel);
            var moduleLoader = NinjectDependencyInjectionFactory.CreateModuleLoader(kernel, configuration, logger);

            // Load test modules
            moduleLoader.LoadModule<TestModule>();
            moduleLoader.LoadModule<AnotherTestModule>();

            // Resolve and use services
            var testService = container.GetService<ITestService>();
            logger.LogInformation("Resolved service: {ServiceType}", testService.GetType().Name);
            logger.LogInformation("Service value: {Value}", testService.GetValue());

            // Test scoped services
            using (var scope = container.CreateScope())
            {
                var scopedService = scope.GetService<IDependentService>();
                logger.LogInformation("Scoped service value: {Value}", scopedService.GetCombinedValue());
            }

            // Test factory services
            var factoryService = container.GetService<IFactoryService>();
            logger.LogInformation("Factory service value: {Value}", factoryService.GetFactoryValue());

            kernel.Dispose();
        }

        /// <summary>
        /// Example 2: Using the Revo startup class
        /// </summary>
        static async Task RunRevoStartupExample(IConfiguration configuration, ILogger logger)
        {
            logger.LogInformation("=== Example 2: Revo Startup Class ===");

            // Create Revo configuration
            var revoConfiguration = new RevoConfiguration();
            revoConfiguration.ConfigureCoreWithNewDi();

            // Create Revo startup
            var startup = revoConfiguration.CreateRevoStartup(logger);

            // Configure with Ninject
            var container = startup.ConfigureWithNinject();

            // Load modules
            startup.LoadModulesFromCurrentDomain();

            // Run application initialization
            startup.RunApplicationInitialization();

            // Use the container
            var testService = container.GetService<ITestService>();
            logger.LogInformation("Revo startup service value: {Value}", testService.GetValue());

            // Run application shutdown
            startup.RunApplicationShutdown();
        }

        /// <summary>
        /// Example 3: Using the unified bootstrapper
        /// </summary>
        static async Task RunUnifiedBootstrapperExample(IConfiguration configuration, ILogger logger)
        {
            logger.LogInformation("=== Example 3: Unified Bootstrapper ===");

            // Create Revo configuration
            var revoConfiguration = new RevoConfiguration();
            revoConfiguration.ConfigureCoreWithNewDi();

            // Create unified bootstrapper with new DI system
            var bootstrapper = revoConfiguration.CreateUnifiedBootstrapperWithNewDi(logger);

            // Configure
            bootstrapper.Configure();

            // Load modules
            var assemblies = new[] { typeof(Program).Assembly };
            bootstrapper.LoadAssemblies(assemblies);

            // Run application initialization
            bootstrapper.RunAppConfigurers();
            bootstrapper.RunAppStartListeners();

            // Use the container
            var testService = bootstrapper.GetService<ITestService>();
            logger.LogInformation("Unified bootstrapper service value: {Value}", testService.GetValue());

            // Run application shutdown
            bootstrapper.RunAppStopListeners();
        }
    }

    // Test services for the example
    public interface ITestService
    {
        string GetValue();
    }

    public class TestService : ITestService
    {
        public string GetValue() => "Hello from TestService!";
    }

    public interface IDependentService
    {
        string GetCombinedValue();
    }

    public class DependentService : IDependentService
    {
        private readonly ITestService _testService;

        public DependentService(ITestService testService)
        {
            _testService = testService;
        }

        public string GetCombinedValue() => $"Dependent: {_testService.GetValue()}";
    }

    public interface IFactoryService
    {
        string GetFactoryValue();
    }

    public class FactoryService : IFactoryService
    {
        public string GetFactoryValue() => "Created by factory!";
    }

    // Test modules
    public class TestModule : NinjectDependencyModule
    {
        public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
        {
            registry.RegisterSingleton<ITestService, TestService>();
            registry.RegisterScoped<IDependentService, DependentService>();
            registry.RegisterFactory<IFactoryService>(provider => new FactoryService(), ServiceLifetime.Transient);
        }
    }

    public class AnotherTestModule : NinjectDependencyModule
    {
        public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
        {
            // This module doesn't register anything, just for demonstration
        }
    }
}
