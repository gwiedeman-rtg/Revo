using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Microsoft;
using Xunit;
using MSDI = Microsoft.Extensions.DependencyInjection;

namespace Revo.DependencyInjection.Microsoft.Tests
{
    /// <summary>
    /// Unit tests for MicrosoftServiceContainer.
    /// </summary>
    public class MicrosoftServiceContainerTests
    {
        [Fact]
        public void Constructor_WithNullServiceProvider_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MicrosoftServiceContainer(null));
        }

        [Fact]
        public void GetService_WithRegisteredService_ShouldReturnService()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);

            // Act
            var result = container.GetService<ITestService>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestService>();
        }

        [Fact]
        public void GetService_WithUnregisteredService_ShouldReturnNull()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);

            // Act
            var result = container.GetService<ITestService>();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetService_WithType_ShouldReturnService()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);

            // Act
            var result = container.GetService(typeof(ITestService));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestService>();
        }

        [Fact]
        public void GetServices_WithMultipleRegistrations_ShouldReturnAllServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();
            services.AddSingleton<ITestService, AnotherTestService>();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);

            // Act
            var results = container.GetServices<ITestService>().ToList();

            // Assert
            results.Should().HaveCount(2);
            results.Should().ContainItemsAssignableTo<ITestService>();
        }

        [Fact]
        public void GetServices_WithType_ShouldReturnAllServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();
            services.AddSingleton<ITestService, AnotherTestService>();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);

            // Act
            var results = container.GetServices(typeof(ITestService)).ToList();

            // Assert
            results.Should().HaveCount(2);
            results.Should().AllBeAssignableTo<ITestService>();
        }

        [Fact]
        public void CreateScope_ShouldReturnServiceScope()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ITestService, TestService>();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);

            // Act
            var scope = container.CreateScope();

            // Assert
            scope.Should().NotBeNull();
            scope.Should().BeOfType<MicrosoftServiceScope>();
        }

        [Fact]
        public void GetServiceOrDefault_WithRegisteredService_ShouldReturnService()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);

            // Act
            var result = container.GetServiceOrDefault<ITestService>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestService>();
        }

        [Fact]
        public void GetServiceOrDefault_WithUnregisteredService_ShouldReturnNull()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);

            // Act
            var result = container.GetServiceOrDefault<ITestService>();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetServiceOrDefault_WithType_ShouldReturnService()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);

            // Act
            var result = container.GetServiceOrDefault(typeof(ITestService));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestService>();
        }

        [Fact]
        public void ServiceProvider_ShouldReturnUnderlyingServiceProvider()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);

            // Act
            var result = container.ServiceProvider;

            // Assert
            result.Should().BeSameAs(serviceProvider);
        }
    }

    // Test interfaces and implementations
    public interface ITestService
    {
        string GetValue();
    }

    public class TestService : ITestService
    {
        public string GetValue() => "TestService";
    }

    public class AnotherTestService : ITestService
    {
        public string GetValue() => "AnotherTestService";
    }
}
