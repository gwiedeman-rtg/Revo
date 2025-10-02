using System;
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
    /// Unit tests for MicrosoftDependencyInjectionFactory.
    /// </summary>
    public class MicrosoftDependencyInjectionFactoryTests
    {
        [Fact]
        public void CreateServiceCollection_ShouldReturnNewServiceCollection()
        {
            // Act
            var result = MicrosoftDependencyInjectionFactory.CreateServiceCollection();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ServiceCollection>();
        }

        [Fact]
        public void CreateServiceContainer_WithServiceCollection_ShouldReturnContainer()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();

            // Act
            var result = MicrosoftDependencyInjectionFactory.CreateServiceContainer(services);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<MicrosoftServiceContainer>();
        }

        [Fact]
        public void CreateServiceContainer_WithNullServiceCollection_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => MicrosoftDependencyInjectionFactory.CreateServiceContainer((IServiceCollection)null));
        }

        [Fact]
        public void CreateServiceContainer_WithServiceProvider_ShouldReturnContainer()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var result = MicrosoftDependencyInjectionFactory.CreateServiceContainer(serviceProvider);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<MicrosoftServiceContainer>();
        }

        [Fact]
        public void CreateServiceContainer_WithNullServiceProvider_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => MicrosoftDependencyInjectionFactory.CreateServiceContainer((IServiceProvider)null));
        }

        [Fact]
        public void CreateServiceRegistry_WithServiceCollection_ShouldReturnRegistry()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = MicrosoftDependencyInjectionFactory.CreateServiceRegistry(services);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<MicrosoftServiceRegistry>();
        }

        [Fact]
        public void CreateServiceRegistry_WithNullServiceCollection_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => MicrosoftDependencyInjectionFactory.CreateServiceRegistry(null));
        }

        [Fact]
        public void CreateModuleLoader_WithValidParameters_ShouldReturnModuleLoader()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MicrosoftDependencyInjectionFactoryTests>();

            // Act
            var result = MicrosoftDependencyInjectionFactory.CreateModuleLoader(services, configuration, logger);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<MicrosoftModuleLoader>();
        }

        [Fact]
        public void CreateModuleLoader_WithNullServices_ShouldThrowArgumentNullException()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MicrosoftDependencyInjectionFactoryTests>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => MicrosoftDependencyInjectionFactory.CreateModuleLoader(null, configuration, logger));
        }

        [Fact]
        public void CreateModuleLoader_WithNullConfiguration_ShouldThrowArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MicrosoftDependencyInjectionFactoryTests>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => MicrosoftDependencyInjectionFactory.CreateModuleLoader(services, null, logger));
        }

        [Fact]
        public void CreateModuleLoader_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => MicrosoftDependencyInjectionFactory.CreateModuleLoader(services, configuration, null));
        }

        [Fact]
        public void CreateCompleteSetup_WithValidParameters_ShouldReturnAllComponents()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MicrosoftDependencyInjectionFactoryTests>();

            // Act
            var (services, container, moduleLoader) = MicrosoftDependencyInjectionFactory.CreateCompleteSetup(configuration, logger);

            // Assert
            services.Should().NotBeNull();
            services.Should().BeOfType<ServiceCollection>();
            
            container.Should().NotBeNull();
            container.Should().BeOfType<MicrosoftServiceContainer>();
            
            moduleLoader.Should().NotBeNull();
            moduleLoader.Should().BeOfType<MicrosoftModuleLoader>();
        }

        [Fact]
        public void CreateCompleteSetup_WithNullConfiguration_ShouldThrowArgumentNullException()
        {
            // Arrange
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MicrosoftDependencyInjectionFactoryTests>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => MicrosoftDependencyInjectionFactory.CreateCompleteSetup(null, logger));
        }

        [Fact]
        public void CreateCompleteSetup_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => MicrosoftDependencyInjectionFactory.CreateCompleteSetup(configuration, null));
        }

        [Fact]
        public void CreateCompleteSetup_WithConfigureServices_ShouldCallConfigureServices()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MicrosoftDependencyInjectionFactoryTests>();
            var configureServicesCalled = false;

            // Act
            var (services, container, moduleLoader) = MicrosoftDependencyInjectionFactory.CreateCompleteSetup(
                configuration, 
                logger, 
                s => 
                {
                    s.AddSingleton<ITestService, TestService>();
                    configureServicesCalled = true;
                });

            // Assert
            configureServicesCalled.Should().BeTrue();
            services.Should().Contain(s => s.ServiceType == typeof(ITestService));
        }
    }
}
