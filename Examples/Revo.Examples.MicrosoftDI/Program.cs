using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Microsoft;

namespace Revo.Examples.MicrosoftDI
{
    /// <summary>
    /// Example application demonstrating the Microsoft DI implementation of Revo dependency injection system.
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Revo Microsoft DI Example");
            Console.WriteLine("=========================");
            Console.WriteLine();

            // Create configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            // Create logger
            var loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Information));
            var logger = loggerFactory.CreateLogger<Program>();

            try
            {
                // Example 1: Using Microsoft DI directly
                await RunDirectMicrosoftDiExample(configuration, logger);

                Console.WriteLine();

                // Example 2: Using the factory methods
                await RunFactoryExample(configuration, logger);

                Console.WriteLine();

                // Example 3: Using the complete setup
                await RunCompleteSetupExample(configuration, logger);
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
        /// Example 1: Using Microsoft DI directly
        /// </summary>
        static async Task RunDirectMicrosoftDiExample(IConfiguration configuration, ILogger logger)
        {
            logger.LogInformation("=== Example 1: Direct Microsoft DI Usage ===");

            // Create Microsoft service collection
            var services = new ServiceCollection();

            // Create service registry and register services
            var registry = MicrosoftDependencyInjectionFactory.CreateServiceRegistry(services);
            MicrosoftServiceRegistryExtensions.RegisterSingleton<ITestService, TestService>(registry);
            MicrosoftServiceRegistryExtensions.RegisterScoped<IDependentService, DependentService>(registry);
            registry.RegisterFactory<IFactoryService>(provider => new FactoryService("Factory Created Value"), Revo.DependencyInjection.Core.ServiceLifetime.Transient);

            // Create service container
            var container = MicrosoftDependencyInjectionFactory.CreateServiceContainer(services);

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
        }

        /// <summary>
        /// Example 2: Using the factory methods
        /// </summary>
        static async Task RunFactoryExample(IConfiguration configuration, ILogger logger)
        {
            logger.LogInformation("=== Example 2: Factory Methods ===");

            // Create service collection
            var services = MicrosoftDependencyInjectionFactory.CreateServiceCollection();

            // Register services using Microsoft DI directly
            services.AddSingleton<ITestService, TestService>();
            services.AddScoped<IDependentService, DependentService>();
            services.AddTransient<IFactoryService>(provider => new FactoryService("Factory Method Created"));

            // Create container and module loader
            var container = MicrosoftDependencyInjectionFactory.CreateServiceContainer(services);
            var moduleLoader = MicrosoftDependencyInjectionFactory.CreateModuleLoader(services, configuration, logger);

            // Load test modules
            moduleLoader.LoadModule<TestModule>();

            // Use the container
            var testService = container.GetService<ITestService>();
            logger.LogInformation("Factory method service value: {Value}", testService.GetValue());
        }

        /// <summary>
        /// Example 3: Using the complete setup
        /// </summary>
        static async Task RunCompleteSetupExample(IConfiguration configuration, ILogger logger)
        {
            logger.LogInformation("=== Example 3: Complete Setup ===");

            // Create complete setup
            var (services, container, moduleLoader) = MicrosoftDependencyInjectionFactory.CreateCompleteSetup(
                configuration, 
                logger, 
                s => s.AddSingleton<ITestService, TestService>());

            // Load modules
            moduleLoader.LoadModule<TestModule>();
            moduleLoader.LoadModule<AnotherTestModule>();

            // Use the container
            var testService = container.GetService<ITestService>();
            logger.LogInformation("Complete setup service value: {Value}", testService.GetValue());

            // Test multiple service registrations
            var allServices = container.GetServices<ITestService>();
            logger.LogInformation("Total registered services: {Count}", allServices.Count());
        }
    }

    // Test services for the example
    public interface ITestService
    {
        string GetValue();
    }

    public class TestService : ITestService
    {
        public string GetValue() => "Hello from Microsoft DI TestService!";
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
        private readonly string _value;

        public FactoryService(string value)
        {
            _value = value;
        }

        public string GetFactoryValue() => _value;
    }

    // Test modules
    public class TestModule : MicrosoftDependencyModule
    {
        public override void Configure(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            services.AddSingleton<ITestService, TestService>();
            services.AddScoped<IDependentService, DependentService>();
            services.AddTransient<IFactoryService>(provider => new FactoryService("Module Created Value"));
        }
    }

    public class AnotherTestModule : MicrosoftDependencyModule
    {
        public override void Configure(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            // This module doesn't register anything, just for demonstration
            logger.LogInformation("AnotherTestModule configured");
        }
    }
}
