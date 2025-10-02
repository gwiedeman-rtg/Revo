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
    /// Unit tests for MicrosoftServiceRegistry.
    /// </summary>
    public class MicrosoftServiceRegistryTests
    {
        [Fact]
        public void Constructor_WithNullServices_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MicrosoftServiceRegistry(null));
        }

        [Fact]
        public void Register_WithValidTypes_ShouldAddServiceDescriptor()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);

            // Act
            registry.Register<ITestService, TestService>(Revo.DependencyInjection.Core.ServiceLifetime.Singleton);

            // Assert
            services.Should().HaveCount(1);
            var descriptor = services.First();
            descriptor.ServiceType.Should().Be(typeof(ITestService));
            descriptor.ImplementationType.Should().Be(typeof(TestService));
            descriptor.Lifetime.Should().Be(MSDI.ServiceLifetime.Singleton);
        }

        [Fact]
        public void Register_WithType_ShouldAddServiceDescriptor()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);

            // Act
            registry.Register(typeof(ITestService), typeof(TestService), Revo.DependencyInjection.Core.ServiceLifetime.Scoped);

            // Assert
            services.Should().HaveCount(1);
            var descriptor = services.First();
            descriptor.ServiceType.Should().Be(typeof(ITestService));
            descriptor.ImplementationType.Should().Be(typeof(TestService));
            descriptor.Lifetime.Should().Be(MSDI.ServiceLifetime.Scoped);
        }

        [Fact]
        public void RegisterInstance_ShouldAddInstanceDescriptor()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);
            var instance = new TestService();

            // Act
            registry.RegisterInstance<ITestService>(instance);

            // Assert
            services.Should().HaveCount(1);
            var descriptor = services.First();
            descriptor.ServiceType.Should().Be(typeof(ITestService));
            descriptor.ImplementationInstance.Should().Be(instance);
            descriptor.Lifetime.Should().Be(MSDI.ServiceLifetime.Singleton);
        }

        [Fact]
        public void RegisterFactory_ShouldAddFactoryDescriptor()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);
            Func<IServiceProvider, ITestService> factory = provider => new TestService();

            // Act
            registry.RegisterFactory(factory, Revo.DependencyInjection.Core.ServiceLifetime.Transient);

            // Assert
            services.Should().HaveCount(1);
            var descriptor = services.First();
            descriptor.ServiceType.Should().Be(typeof(ITestService));
            descriptor.ImplementationFactory.Should().NotBeNull();
            descriptor.Lifetime.Should().Be(MSDI.ServiceLifetime.Transient);
        }

        [Fact]
        public void RegisterMultiple_ShouldAddMultipleDescriptors()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);
            var serviceTypes = new[] { typeof(ITestService), typeof(IAnotherService) };

            // Act
            registry.RegisterMultiple(serviceTypes, typeof(MultiInterfaceService), Revo.DependencyInjection.Core.ServiceLifetime.Singleton);

            // Assert
            services.Should().HaveCount(2);
            services.Should().AllSatisfy(descriptor => 
                descriptor.ImplementationType.Should().Be(typeof(MultiInterfaceService)));
            services.Should().AllSatisfy(descriptor => 
                descriptor.Lifetime.Should().Be(MSDI.ServiceLifetime.Singleton));
        }

        [Fact]
        public void RegisterSelf_ShouldAddSelfDescriptor()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);

            // Act
            registry.RegisterSelf<TestService>(Revo.DependencyInjection.Core.ServiceLifetime.Scoped);

            // Assert
            services.Should().HaveCount(1);
            var descriptor = services.First();
            descriptor.ServiceType.Should().Be(typeof(TestService));
            descriptor.ImplementationType.Should().Be(typeof(TestService));
            descriptor.Lifetime.Should().Be(MSDI.ServiceLifetime.Scoped);
        }

        [Fact]
        public void Rebind_ShouldRemoveExistingAndAddNew()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);
            registry.Register<ITestService, TestService>(Revo.DependencyInjection.Core.ServiceLifetime.Singleton);

            // Act
            registry.Rebind<ITestService, AnotherTestService>(Revo.DependencyInjection.Core.ServiceLifetime.Transient);

            // Assert
            services.Should().HaveCount(1);
            var descriptor = services.First();
            descriptor.ServiceType.Should().Be(typeof(ITestService));
            descriptor.ImplementationType.Should().Be(typeof(AnotherTestService));
            descriptor.Lifetime.Should().Be(MSDI.ServiceLifetime.Transient);
        }

        [Fact]
        public void Register_ShouldReturnRegistryForChaining()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);

            // Act
            var result = registry.Register<ITestService, TestService>(Revo.DependencyInjection.Core.ServiceLifetime.Singleton);

            // Assert
            result.Should().BeSameAs(registry);
        }

        [Fact]
        public void RegisterInstance_ShouldReturnRegistryForChaining()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);
            var instance = new TestService();

            // Act
            var result = registry.RegisterInstance<ITestService>(instance);

            // Assert
            result.Should().BeSameAs(registry);
        }

        [Fact]
        public void RegisterFactory_ShouldReturnRegistryForChaining()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);
            Func<IServiceProvider, ITestService> factory = provider => new TestService();

            // Act
            var result = registry.RegisterFactory(factory, Revo.DependencyInjection.Core.ServiceLifetime.Transient);

            // Assert
            result.Should().BeSameAs(registry);
        }

        [Fact]
        public void RegisterMultiple_ShouldReturnRegistryForChaining()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);
            var serviceTypes = new[] { typeof(ITestService) };

            // Act
            var result = registry.RegisterMultiple(serviceTypes, typeof(TestService), Revo.DependencyInjection.Core.ServiceLifetime.Singleton);

            // Assert
            result.Should().BeSameAs(registry);
        }

        [Fact]
        public void RegisterSelf_ShouldReturnRegistryForChaining()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);

            // Act
            var result = registry.RegisterSelf<TestService>(Revo.DependencyInjection.Core.ServiceLifetime.Singleton);

            // Assert
            result.Should().BeSameAs(registry);
        }

        [Fact]
        public void Rebind_ShouldReturnRegistryForChaining()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);

            // Act
            var result = registry.Rebind<ITestService, TestService>(Revo.DependencyInjection.Core.ServiceLifetime.Singleton);

            // Assert
            result.Should().BeSameAs(registry);
        }

        [Fact]
        public void Services_ShouldReturnUnderlyingServiceCollection()
        {
            // Arrange
            var services = new ServiceCollection();
            var registry = new MicrosoftServiceRegistry(services);

            // Act
            var result = registry.Services;

            // Assert
            result.Should().BeSameAs(services);
        }
    }

    // Additional test interfaces
    public interface IAnotherService
    {
        string GetValue();
    }

    public class MultiInterfaceService : ITestService, IAnotherService
    {
        public string GetValue() => "MultiInterfaceService";
    }
}
