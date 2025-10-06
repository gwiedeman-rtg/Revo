using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Revo.Core.Core;
using Xunit;

namespace Revo.DependencyInjection.Simplified.Tests
{
    public class MicrosoftServiceLocatorTests
    {
        [Fact]
        public void Get_WithRegisteredService_ReturnsService()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();
            var serviceProvider = services.BuildServiceProvider();
            var serviceLocator = new MicrosoftServiceLocator(serviceProvider);

            // Act
            var result = serviceLocator.Get<ITestService>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestService>();
        }

        [Fact]
        public void Get_WithUnregisteredService_ReturnsNull()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var serviceLocator = new MicrosoftServiceLocator(serviceProvider);

            // Act
            var result = serviceLocator.Get<ITestService>();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Get_WithType_ReturnsService()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();
            var serviceProvider = services.BuildServiceProvider();
            var serviceLocator = new MicrosoftServiceLocator(serviceProvider);

            // Act
            var result = serviceLocator.Get(typeof(ITestService));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestService>();
        }

        [Fact]
        public void GetAll_WithRegisteredServices_ReturnsAllServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();
            services.AddSingleton<ITestService, AnotherTestService>();
            var serviceProvider = services.BuildServiceProvider();
            var serviceLocator = new MicrosoftServiceLocator(serviceProvider);

            // Act
            var result = serviceLocator.GetAll<ITestService>();

            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainItemsAssignableTo<ITestService>();
        }

        [Fact]
        public void GetAll_WithType_ReturnsAllServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ITestService, TestService>();
            services.AddSingleton<ITestService, AnotherTestService>();
            var serviceProvider = services.BuildServiceProvider();
            var serviceLocator = new MicrosoftServiceLocator(serviceProvider);

            // Act
            var result = serviceLocator.GetAll(typeof(ITestService));

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(x => x.Should().BeAssignableTo<ITestService>());
        }

        [Fact]
        public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MicrosoftServiceLocator(null));
        }
    }

    public interface ITestService
    {
        string Name { get; }
    }

    public class TestService : ITestService
    {
        public string Name => "TestService";
    }

    public class AnotherTestService : ITestService
    {
        public string Name => "AnotherTestService";
    }
}

