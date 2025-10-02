using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Tests for the Ninject service container implementation.
    /// </summary>
    public class NinjectServiceContainerTests : IDisposable
    {
        private readonly IKernel _kernel;
        private readonly IServiceContainer _container;

        public NinjectServiceContainerTests()
        {
            _kernel = new StandardKernel();
            _container = new NinjectServiceContainer(_kernel);
        }

        [Fact]
        public void GetService_WithRegisteredService_ShouldReturnService()
        {
            // Arrange
            _kernel.Bind<ITestService>().To<TestService>().InSingletonScope();

            // Act
            var service = _container.GetService<ITestService>();

            // Assert
            service.Should().NotBeNull();
            service.Should().BeOfType<TestService>();
        }

        [Fact]
        public void GetService_WithUnregisteredService_ShouldThrowException()
        {
            // Act & Assert
            _container.Invoking(c => c.GetService<ITestService>())
                .Should().Throw<ActivationException>();
        }

        [Fact]
        public void GetServiceOrDefault_WithRegisteredService_ShouldReturnService()
        {
            // Arrange
            _kernel.Bind<ITestService>().To<TestService>().InSingletonScope();

            // Act
            var service = _container.GetServiceOrDefault<ITestService>();

            // Assert
            service.Should().NotBeNull();
            service.Should().BeOfType<TestService>();
        }

        [Fact]
        public void GetServiceOrDefault_WithUnregisteredService_ShouldReturnNull()
        {
            // Act
            var service = _container.GetServiceOrDefault<ITestService>();

            // Assert
            service.Should().BeNull();
        }

        [Fact]
        public void GetServices_WithMultipleRegistrations_ShouldReturnAllServices()
        {
            // Arrange
            _kernel.Bind<ITestService>().To<TestService>().InSingletonScope();
            _kernel.Bind<ITestService>().To<MultiInterfaceService>().InSingletonScope();

            // Act
            var services = _container.GetServices<ITestService>().ToList();

            // Assert
            services.Should().HaveCount(2);
            services.Should().Contain(s => s is TestService);
            services.Should().Contain(s => s is MultiInterfaceService);
        }

        [Fact]
        public void CreateScope_ShouldReturnServiceScope()
        {
            // Act
            var scope = _container.CreateScope();

            // Assert
            scope.Should().NotBeNull();
            scope.Should().BeOfType<NinjectServiceScope>();
        }

        [Fact]
        public void CreateScope_ShouldAllowServiceResolution()
        {
            // Arrange
            _kernel.Bind<ITestService>().To<TestService>().InSingletonScope();

            // Act
            using var scope = _container.CreateScope();
            var service = scope.GetService<ITestService>();

            // Assert
            service.Should().NotBeNull();
            service.Should().BeOfType<TestService>();
        }

        public void Dispose()
        {
            _kernel?.Dispose();
        }
    }
}
