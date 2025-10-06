using System;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Revo.Core.Core;
using Xunit;

namespace Revo.DependencyInjection.Simplified.Tests
{
    public class CoreRevoModuleTests
    {
        [Fact]
        public void Configure_RegistersServiceLocator()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CoreRevoModule>();
            var module = new CoreRevoModule();

            // Act
            module.Configure(services, configuration, logger);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var serviceLocator = serviceProvider.GetService<IServiceLocator>();
            serviceLocator.Should().NotBeNull();
            serviceLocator.Should().BeOfType<MicrosoftServiceLocator>();
        }

        [Fact]
        public void Configure_WithNullServices_ThrowsArgumentNullException()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CoreRevoModule>();
            var module = new CoreRevoModule();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => module.Configure(null, configuration, logger));
        }

        [Fact]
        public void Configure_WithNullConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CoreRevoModule>();
            var module = new CoreRevoModule();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => module.Configure(services, null, logger));
        }

        [Fact]
        public void Configure_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var module = new CoreRevoModule();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => module.Configure(services, configuration, null));
        }
    }
}

