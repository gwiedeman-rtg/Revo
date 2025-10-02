using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Microsoft;
using Xunit;
using MSDI = Microsoft.Extensions.DependencyInjection;

namespace Revo.DependencyInjection.Microsoft.Tests
{
    /// <summary>
    /// Unit tests for MicrosoftModuleLoader.
    /// </summary>
    public class MicrosoftModuleLoaderTests
    {
        private readonly IServiceCollection _services;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly MicrosoftModuleLoader _moduleLoader;

        public MicrosoftModuleLoaderTests()
        {
            _services = new ServiceCollection();
            _configuration = new ConfigurationBuilder().Build();
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MicrosoftModuleLoaderTests>();
            _moduleLoader = new MicrosoftModuleLoader(_services, _configuration, _logger);
        }

        [Fact]
        public void Constructor_WithNullServices_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MicrosoftModuleLoader(null, _configuration, _logger));
        }

        [Fact]
        public void Constructor_WithNullConfiguration_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MicrosoftModuleLoader(_services, null, _logger));
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MicrosoftModuleLoader(_services, _configuration, null));
        }

        [Fact]
        public void LoadModule_WithValidModule_ShouldLoadModule()
        {
            // Act
            _moduleLoader.LoadModule(typeof(TestModule));

            // Assert
            _moduleLoader.LoadedModules.Should().HaveCount(1);
            _moduleLoader.LoadedModules.First().Should().BeOfType<TestModule>();
        }

        [Fact]
        public void LoadModule_WithType_ShouldLoadModule()
        {
            // Act
            _moduleLoader.LoadModule(typeof(TestModule));

            // Assert
            _moduleLoader.LoadedModules.Should().HaveCount(1);
            _moduleLoader.LoadedModules.First().Should().BeOfType<TestModule>();
        }

        [Fact]
        public void LoadModule_WithInvalidType_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _moduleLoader.LoadModule(typeof(string)));
        }

        [Fact]
        public void LoadModule_WithNullType_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _moduleLoader.LoadModule((Type)null));
        }

        [Fact]
        public void LoadModule_Twice_ShouldNotLoadTwice()
        {
            // Act
            _moduleLoader.LoadModule(typeof(TestModule));
            _moduleLoader.LoadModule(typeof(TestModule));

            // Assert
            _moduleLoader.LoadedModules.Should().HaveCount(1);
        }

        [Fact]
        public void LoadModulesFromAssembly_ShouldLoadAllModules()
        {
            // Act
            _moduleLoader.LoadModulesFromAssembly(typeof(TestModule).Assembly);

            // Assert
            _moduleLoader.LoadedModules.Should().NotBeEmpty();
            _moduleLoader.LoadedModules.Should().Contain(m => m is TestModule);
        }

        [Fact]
        public void LoadModulesFromAssembly_WithNullAssembly_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _moduleLoader.LoadModulesFromAssembly(null));
        }

        [Fact]
        public void LoadModulesFromAssemblies_ShouldLoadAllModules()
        {
            // Arrange
            var assemblies = new[] { typeof(TestModule).Assembly };

            // Act
            _moduleLoader.LoadModulesFromAssemblies(assemblies);

            // Assert
            _moduleLoader.LoadedModules.Should().NotBeEmpty();
        }

        [Fact]
        public void LoadModulesFromAssemblies_WithNullAssemblies_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _moduleLoader.LoadModulesFromAssemblies(null));
        }

        [Fact]
        public void LoadedModules_ShouldReturnReadOnlyList()
        {
            // Act
            var result = _moduleLoader.LoadedModules;

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IReadOnlyList<IDependencyModule>>();
        }

        [Fact]
        public void LoadModule_WithServicesRegistered_ShouldResolveServices()
        {
            // Act
            _moduleLoader.LoadModule(typeof(TestModule));

            // Build the service provider and test resolution
            var serviceProvider = _services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);

            // Assert
            var testService = container.GetService<ITestService>();
            testService.Should().NotBeNull();
            testService.Should().BeOfType<TestService>();
        }
    }

    // Test modules
    public class TestModule : MicrosoftDependencyModule
    {
        public override void Configure(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            services.AddSingleton<ITestService, TestService>();
            services.AddScoped<IDependentService, DependentService>();
        }
    }

    public class AnotherTestModule : MicrosoftDependencyModule
    {
        public override void Configure(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            // This module doesn't register anything, just for demonstration
        }
    }

    // Additional test interfaces
    public interface IDependentService
    {
        string GetValue();
    }

    public class DependentService : IDependentService
    {
        public string GetValue() => "DependentService";
    }
}
