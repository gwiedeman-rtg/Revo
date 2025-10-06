using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Revo.Core.Configuration;
using Revo.Core.Core;
using Xunit;
using MSConfig = Microsoft.Extensions.Configuration;

namespace Revo.DependencyInjection.Simplified.Tests
{
    public class RevoMicrosoftDIStartupTests
    {
        [Fact]
        public void Configure_RegistersServiceLocator()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<RevoMicrosoftDIStartup>();
            var startup = new RevoMicrosoftDIStartup(services, configuration, logger);

            // Act
            startup.Configure();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var serviceLocator = serviceProvider.GetService<IServiceLocator>();
            serviceLocator.Should().NotBeNull();
            serviceLocator.Should().BeOfType<MicrosoftServiceLocator>();
        }

        [Fact]
        public void Configure_RegistersConfiguration()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<RevoMicrosoftDIStartup>();
            var startup = new RevoMicrosoftDIStartup(services, configuration, logger);

            // Act
            startup.Configure();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var registeredConfiguration = serviceProvider.GetService<MSConfig.IConfiguration>();
            registeredConfiguration.Should().NotBeNull();
            registeredConfiguration.Should().BeSameAs(configuration);
        }

        [Fact]
        public void Configure_RegistersLogger()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<RevoMicrosoftDIStartup>();
            var startup = new RevoMicrosoftDIStartup(services, configuration, logger);

            // Act
            startup.Configure();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var registeredLogger = serviceProvider.GetService<ILogger>();
            registeredLogger.Should().NotBeNull();
            registeredLogger.Should().BeSameAs(logger);
        }

        [Fact]
        public void Constructor_WithNullServices_ThrowsArgumentNullException()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<RevoMicrosoftDIStartup>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RevoMicrosoftDIStartup(null, configuration, logger));
        }

        [Fact]
        public void Constructor_WithNullConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<RevoMicrosoftDIStartup>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RevoMicrosoftDIStartup(services, null, logger));
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RevoMicrosoftDIStartup(services, configuration, null));
        }
    }
}
