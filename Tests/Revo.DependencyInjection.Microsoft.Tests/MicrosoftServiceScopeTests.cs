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
    /// Unit tests for MicrosoftServiceScope.
    /// </summary>
    public class MicrosoftServiceScopeTests
    {
        [Fact]
        public void Constructor_WithNullContainer_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MicrosoftServiceScope(null));
        }

        [Fact]
        public void GetService_WithScopedService_ShouldReturnService()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ITestService, TestService>();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);
            var scope = new MicrosoftServiceScope(container);

            // Act
            var result = scope.GetService<ITestService>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestService>();
        }

        [Fact]
        public void GetService_WithType_ShouldReturnService()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ITestService, TestService>();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);
            var scope = new MicrosoftServiceScope(container);

            // Act
            var result = scope.GetService(typeof(ITestService));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TestService>();
        }

        [Fact]
        public void GetServices_WithMultipleRegistrations_ShouldReturnAllServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<ITestService, AnotherTestService>();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);
            var scope = new MicrosoftServiceScope(container);

            // Act
            var results = scope.GetServices<ITestService>().ToList();

            // Assert
            results.Should().HaveCount(2);
            results.Should().ContainItemsAssignableTo<ITestService>();
        }

        [Fact]
        public void GetServices_WithType_ShouldReturnAllServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<ITestService, AnotherTestService>();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);
            var scope = new MicrosoftServiceScope(container);

            // Act
            var results = scope.GetServices(typeof(ITestService)).ToList();

            // Assert
            results.Should().HaveCount(2);
            results.Should().AllBeAssignableTo<ITestService>();
        }

        [Fact]
        public void ServiceProvider_ShouldReturnScopeServiceProvider()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);
            var scope = new MicrosoftServiceScope(container);

            // Act
            var result = scope.ServiceProvider;

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeSameAs(serviceProvider);
        }

        [Fact]
        public void Dispose_ShouldDisposeScope()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);
            var scope = new MicrosoftServiceScope(container);

            // Act & Assert
            scope.Invoking(s => s.Dispose()).Should().NotThrow();
        }

        [Fact]
        public void Dispose_MultipleTimes_ShouldNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);
            var scope = new MicrosoftServiceScope(container);

            // Act & Assert
            scope.Invoking(s => s.Dispose()).Should().NotThrow();
            scope.Invoking(s => s.Dispose()).Should().NotThrow();
        }

        [Fact]
        public void ScopedServices_ShouldBeSameInstanceWithinScope()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ITestService, TestService>();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);

            using (var scope = new MicrosoftServiceScope(container))
            {
                // Act
                var service1 = scope.GetService<ITestService>();
                var service2 = scope.GetService<ITestService>();

                // Assert
                service1.Should().BeSameAs(service2);
            }
        }

        [Fact]
        public void ScopedServices_ShouldBeDifferentInstancesAcrossScopes()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<ITestService, TestService>();
            var serviceProvider = services.BuildServiceProvider();
            var container = new MicrosoftServiceContainer(serviceProvider);

            ITestService service1, service2;

            using (var scope1 = new MicrosoftServiceScope(container))
            {
                service1 = scope1.GetService<ITestService>();
            }

            using (var scope2 = new MicrosoftServiceScope(container))
            {
                service2 = scope2.GetService<ITestService>();
            }

            // Assert
            service1.Should().NotBeSameAs(service2);
        }
    }
}
