using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ninject;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Ninject;
using Xunit;

namespace Revo.DependencyInjection.Tests
{
    /// <summary>
    /// Tests for the Ninject module loader implementation.
    /// </summary>
    public class NinjectModuleLoaderTests : IDisposable
    {
        private readonly IKernel _kernel;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IModuleLoader _moduleLoader;

        public NinjectModuleLoaderTests()
        {
            _kernel = new StandardKernel();
            _configuration = new ConfigurationBuilder().Build();
            _logger = new LoggerFactory().CreateLogger<TestModule>();
            _moduleLoader = new NinjectModuleLoader(_kernel, _configuration, _logger);
        }

        [Fact]
        public void LoadModule_WithValidModule_ShouldLoadModule()
        {
            // Act
            _moduleLoader.LoadModule<TestModule>();

            // Assert
            _moduleLoader.IsModuleLoaded<TestModule>().Should().BeTrue();
            _moduleLoader.IsModuleLoaded("TestModule").Should().BeTrue();
        }

        [Fact]
        public void LoadModule_WithModuleInstance_ShouldLoadModule()
        {
            // Arrange
            var module = new TestModule();

            // Act
            _moduleLoader.LoadModule(module);

            // Assert
            _moduleLoader.IsModuleLoaded<TestModule>().Should().BeTrue();
            _moduleLoader.IsModuleLoaded("TestModule").Should().BeTrue();
        }

        [Fact]
        public void LoadModule_WithConstructorArgs_ShouldLoadModule()
        {
            // Act
            _moduleLoader.LoadModule<ConfigurationTestModule>("Test Configuration");

            // Assert
            _moduleLoader.IsModuleLoaded<ConfigurationTestModule>().Should().BeTrue();
        }

        [Fact]
        public void LoadModules_WithAssembly_ShouldLoadAllModules()
        {
            // Arrange
            var assembly = Assembly.GetExecutingAssembly();

            // Act
            _moduleLoader.LoadModules(new[] { assembly });

            // Assert
            _moduleLoader.IsModuleLoaded<TestModule>().Should().BeTrue();
            _moduleLoader.IsModuleLoaded<AnotherTestModule>().Should().BeFalse(); // Disabled by default
        }

        [Fact]
        public void LoadModule_WithServicesRegistered_ShouldResolveServices()
        {
            // Act
            _moduleLoader.LoadModule<TestModule>();

            // Assert
            var testService = _kernel.Get<ITestService>();
            var anotherTestService = _kernel.Get<IAnotherTestService>();
            var dependentService = _kernel.Get<IDependentService>();

            testService.Should().NotBeNull();
            anotherTestService.Should().NotBeNull();
            dependentService.Should().NotBeNull();
        }

        [Fact]
        public void LoadModule_Twice_ShouldNotLoadTwice()
        {
            // Act
            _moduleLoader.LoadModule<TestModule>();
            _moduleLoader.LoadModule<TestModule>();

            // Assert
            var loadedModules = _moduleLoader.GetLoadedModules().ToList();
            loadedModules.Should().HaveCount(1);
            loadedModules.Should().Contain(m => m is TestModule);
        }

        [Fact]
        public void GetLoadedModules_ShouldReturnLoadedModules()
        {
            // Arrange
            _moduleLoader.LoadModule<TestModule>();
            _moduleLoader.LoadModule<ConfigurationTestModule>("Test");

            // Act
            var loadedModules = _moduleLoader.GetLoadedModules().ToList();

            // Assert
            loadedModules.Should().HaveCount(2);
            loadedModules.Should().Contain(m => m is TestModule);
            loadedModules.Should().Contain(m => m is ConfigurationTestModule);
        }

        public void Dispose()
        {
            _kernel?.Dispose();
        }
    }
}
