using System;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Ninject;

namespace Revo.DependencyInjection.Tests
{
    /// <summary>
    /// Test module for dependency injection testing.
    /// </summary>
    public class TestModule : NinjectDependencyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestModule"/> class.
        /// </summary>
        public TestModule()
        {
        }

        /// <summary>
        /// Configures the services for this module.
        /// </summary>
        /// <param name="registry">The service registry to register services with.</param>
        /// <param name="context">The module configuration context.</param>
        public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
        {
            // Register service with multiple interfaces (this covers both ITestService and IAnotherTestService)
            registry.RegisterMultiple(
                new[] { typeof(ITestService), typeof(IAnotherTestService) },
                typeof(MultiInterfaceService),
                ServiceLifetime.Scoped);
            
            // Register dependent service
            registry.RegisterScoped<IDependentService, DependentService>();
            
            // Register factory service
            registry.RegisterFactory<IFactoryService>(
                provider => new FactoryService("Factory Created Value"),
                ServiceLifetime.Transient);
        }
    }

    /// <summary>
    /// Another test module for dependency injection testing.
    /// </summary>
    [AutoLoadModule(false)]
    public class AnotherTestModule : NinjectDependencyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnotherTestModule"/> class.
        /// </summary>
        public AnotherTestModule()
        {
        }

        /// <summary>
        /// Configures the services for this module.
        /// </summary>
        /// <param name="registry">The service registry to register services with.</param>
        /// <param name="context">The module configuration context.</param>
        public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
        {
            // This module is disabled by default
            registry.RegisterSingleton<ITestService, TestService>();
        }
    }

    /// <summary>
    /// Test module with configuration.
    /// </summary>
    public class ConfigurationTestModule : NinjectDependencyModule
    {
        private readonly string _configurationValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationTestModule"/> class.
        /// </summary>
        /// <param name="configurationValue">The configuration value.</param>
        public ConfigurationTestModule(string configurationValue)
        {
            _configurationValue = configurationValue ?? throw new ArgumentNullException(nameof(configurationValue));
        }

        /// <summary>
        /// Configures the services for this module.
        /// </summary>
        /// <param name="registry">The service registry to register services with.</param>
        /// <param name="context">The module configuration context.</param>
        public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
        {
            registry.RegisterFactory<IFactoryService>(
                provider => new FactoryService(_configurationValue),
                ServiceLifetime.Singleton);
        }
    }
}
