using System;
using System.Linq;
using FluentAssertions;
using Ninject;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Ninject;
using Xunit;

namespace Revo.DependencyInjection.Tests
{
    /// <summary>
    /// Tests for the Ninject service registry implementation.
    /// </summary>
    public class NinjectServiceRegistryTests : IDisposable
    {
        private readonly IKernel _kernel;
        private readonly IServiceRegistry _registry;

        public NinjectServiceRegistryTests()
        {
            _kernel = new StandardKernel();
            _registry = new NinjectServiceRegistry(_kernel);
        }

        [Fact]
        public void Register_WithInterfaceAndImplementation_ShouldRegisterService()
        {
            // Act
            _registry.Register<ITestService, TestService>(ServiceLifetime.Singleton);

            // Assert
            var service = _kernel.Get<ITestService>();
            service.Should().NotBeNull();
            service.Should().BeOfType<TestService>();
        }

        [Fact]
        public void Register_WithTypeAndImplementation_ShouldRegisterService()
        {
            // Act
            _registry.Register(typeof(ITestService), typeof(TestService), ServiceLifetime.Singleton);

            // Assert
            var service = _kernel.Get<ITestService>();
            service.Should().NotBeNull();
            service.Should().BeOfType<TestService>();
        }

        [Fact]
        public void RegisterInstance_ShouldRegisterInstance()
        {
            // Arrange
            var instance = new TestService();

            // Act
            _registry.RegisterInstance<ITestService>(instance);

            // Assert
            var service = _kernel.Get<ITestService>();
            service.Should().BeSameAs(instance);
        }

        [Fact]
        public void RegisterFactory_ShouldRegisterFactory()
        {
            // Act
            _registry.RegisterFactory<ITestService>(
                provider => new TestService(),
                ServiceLifetime.Transient);

            // Assert
            var service = _kernel.Get<ITestService>();
            service.Should().NotBeNull();
            service.Should().BeOfType<TestService>();
        }

        [Fact]
        public void RegisterMultiple_ShouldRegisterMultipleInterfaces()
        {
            // Act
            _registry.RegisterMultiple(
                new[] { typeof(ITestService), typeof(IAnotherTestService) },
                typeof(MultiInterfaceService),
                ServiceLifetime.Singleton);

            // Assert
            var testService = _kernel.Get<ITestService>();
            var anotherTestService = _kernel.Get<IAnotherTestService>();

            testService.Should().NotBeNull();
            testService.Should().BeOfType<MultiInterfaceService>();
            anotherTestService.Should().NotBeNull();
            anotherTestService.Should().BeOfType<MultiInterfaceService>();
            testService.Should().BeSameAs(anotherTestService);
        }

        [Fact]
        public void RegisterSelf_ShouldRegisterSelf()
        {
            // Act
            _registry.RegisterSelf<TestService>(ServiceLifetime.Singleton);

            // Assert
            var service = _kernel.Get<TestService>();
            service.Should().NotBeNull();
            service.Should().BeOfType<TestService>();
        }

        [Fact]
        public void Rebind_ShouldReplaceExistingRegistration()
        {
            // Arrange
            _registry.Register<ITestService, TestService>(ServiceLifetime.Singleton);
            var firstService = _kernel.Get<ITestService>();

            // Act
            _registry.Rebind<ITestService, MultiInterfaceService>(ServiceLifetime.Singleton);

            // Assert
            var secondService = _kernel.Get<ITestService>();
            secondService.Should().NotBeNull();
            secondService.Should().BeOfType<MultiInterfaceService>();
            secondService.Should().NotBeSameAs(firstService);
        }

        [Fact]
        public void Register_WithDifferentLifetimes_ShouldRespectLifetime()
        {
            // Arrange
            _registry.Register<ITestService, TestService>(ServiceLifetime.Singleton);
            _registry.Register<IAnotherTestService, AnotherTestService>(ServiceLifetime.Transient);

            // Act
            var service1 = _kernel.Get<ITestService>();
            var service2 = _kernel.Get<ITestService>();
            var anotherService1 = _kernel.Get<IAnotherTestService>();
            var anotherService2 = _kernel.Get<IAnotherTestService>();

            // Assert
            service1.Should().BeSameAs(service2); // Singleton
            anotherService1.Should().NotBeSameAs(anotherService2); // Transient
        }

        public void Dispose()
        {
            _kernel?.Dispose();
        }
    }
}
