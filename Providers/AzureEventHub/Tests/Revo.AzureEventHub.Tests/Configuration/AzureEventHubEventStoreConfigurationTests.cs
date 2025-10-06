using System;
using FluentAssertions;
using Revo.AzureEventHub.Configuration;
using Xunit;

namespace Revo.AzureEventHub.Tests.Configuration
{
    public class AzureEventHubEventStoreConfigurationTests
    {
        [Fact]
        public void Validate_WithValidConfiguration_ShouldNotThrow()
        {
            // Arrange
            var configuration = new AzureEventHubEventStoreConfiguration
            {
                ConnectionString = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test",
                EventHubName = "test-hub"
            };

            // Act & Assert
            var action = () => configuration.Validate();
            action.Should().NotThrow();
        }

        [Fact]
        public void Validate_WithEmptyConnectionString_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var configuration = new AzureEventHubEventStoreConfiguration
            {
                ConnectionString = "",
                EventHubName = "test-hub"
            };

            // Act & Assert
            var action = () => configuration.Validate();
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("ConnectionString is required");
        }

        [Fact]
        public void Validate_WithNullConnectionString_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var configuration = new AzureEventHubEventStoreConfiguration
            {
                ConnectionString = null!,
                EventHubName = "test-hub"
            };

            // Act & Assert
            var action = () => configuration.Validate();
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("ConnectionString is required");
        }

        [Fact]
        public void Validate_WithEmptyEventHubName_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var configuration = new AzureEventHubEventStoreConfiguration
            {
                ConnectionString = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test",
                EventHubName = ""
            };

            // Act & Assert
            var action = () => configuration.Validate();
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("EventHubName is required");
        }

        [Fact]
        public void Validate_WithNullEventHubName_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var configuration = new AzureEventHubEventStoreConfiguration
            {
                ConnectionString = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test",
                EventHubName = null!
            };

            // Act & Assert
            var action = () => configuration.Validate();
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("EventHubName is required");
        }

        [Fact]
        public void Validate_WithEnableCaptureTrueButNoStorageConnectionString_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var configuration = new AzureEventHubEventStoreConfiguration
            {
                ConnectionString = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test",
                EventHubName = "test-hub",
                EnableCapture = true,
                CaptureStorageConnectionString = null
            };

            // Act & Assert
            var action = () => configuration.Validate();
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("CaptureStorageConnectionString is required when EnableCapture is true");
        }

        [Fact]
        public void Validate_WithEnableCaptureTrueAndStorageConnectionString_ShouldNotThrow()
        {
            // Arrange
            var configuration = new AzureEventHubEventStoreConfiguration
            {
                ConnectionString = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test",
                EventHubName = "test-hub",
                EnableCapture = true,
                CaptureStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=test"
            };

            // Act & Assert
            var action = () => configuration.Validate();
            action.Should().NotThrow();
        }

        [Fact]
        public void DefaultValues_ShouldBeSetCorrectly()
        {
            // Arrange & Act
            var configuration = new AzureEventHubEventStoreConfiguration();

            // Assert
            configuration.ConsumerGroup.Should().Be("$Default");
            configuration.PartitionStrategy.Should().Be(PartitionStrategy.HashByStreamId);
            configuration.MaxBatchSize.Should().Be(100);
            configuration.MaxWaitTime.Should().Be(TimeSpan.FromSeconds(30));
            configuration.EnableCapture.Should().BeFalse();
            configuration.CaptureContainerName.Should().Be("eventhub-capture");
            configuration.RetryPolicy.Should().NotBeNull();
            configuration.RetryPolicy.MaxRetries.Should().Be(3);
            configuration.RetryPolicy.BaseDelay.Should().Be(TimeSpan.FromSeconds(1));
            configuration.RetryPolicy.MaxDelay.Should().Be(TimeSpan.FromMinutes(1));
            configuration.RetryPolicy.UseExponentialBackoff.Should().BeTrue();
        }
    }
}
