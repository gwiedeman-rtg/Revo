using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Revo.Core.Events;
using Revo.Infrastructure.Events;
using Revo.Infrastructure.EventStores;
using Revo.AzureEventHub.Configuration;
using Revo.AzureEventHub.EventStores;
using Xunit;

namespace Revo.AzureEventHub.Tests.EventStores
{
    public class AzureEventHubEventStoreTests
    {
        private readonly Mock<IEventSerializer> mockEventSerializer;
        private readonly Mock<ILogger<AzureEventHubEventStore>> mockLogger;
        private readonly AzureEventHubEventStoreConfiguration configuration;

        public AzureEventHubEventStoreTests()
        {
            mockEventSerializer = new Mock<IEventSerializer>();
            mockLogger = new Mock<ILogger<AzureEventHubEventStore>>();
            
            configuration = new AzureEventHubEventStoreConfiguration
            {
                ConnectionString = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test",
                EventHubName = "test-hub"
            };
        }

        [Fact]
        public void Constructor_WithValidConfiguration_ShouldNotThrow()
        {
            // Act & Assert
            var action = () => new AzureEventHubEventStore(configuration, mockEventSerializer.Object, mockLogger.Object);
            action.Should().NotThrow();
        }

        [Fact]
        public void Constructor_WithNullConfiguration_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var action = () => new AzureEventHubEventStore(null!, mockEventSerializer.Object, mockLogger.Object);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_WithNullEventSerializer_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var action = () => new AzureEventHubEventStore(configuration, null!, mockLogger.Object);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var action = () => new AzureEventHubEventStore(configuration, mockEventSerializer.Object, null!);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void EventSourceName_ShouldReturnCorrectValue()
        {
            // Arrange
            var eventStore = new AzureEventHubEventStore(configuration, mockEventSerializer.Object, mockLogger.Object);

            // Act
            var result = eventStore.EventSourceName;

            // Assert
            result.Should().Be("AzureEventHub.EventStore");
        }

        [Fact]
        public void AddStream_ShouldNotThrow()
        {
            // Arrange
            var eventStore = new AzureEventHubEventStore(configuration, mockEventSerializer.Object, mockLogger.Object);
            var streamId = Guid.NewGuid();

            // Act & Assert
            var action = () => eventStore.AddStream(streamId);
            action.Should().NotThrow();
        }

        [Fact]
        public async Task GetStreamInfoAsync_ForNewStream_ShouldReturnZeroCounts()
        {
            // Arrange
            var eventStore = new AzureEventHubEventStore(configuration, mockEventSerializer.Object, mockLogger.Object);
            var streamId = Guid.NewGuid();

            // Act
            var result = await eventStore.GetStreamInfoAsync(streamId);

            // Assert
            result.StreamId.Should().Be(streamId);
            result.EventCount.Should().Be(0);
            result.LastSequenceNumber.Should().Be(0);
        }

        [Fact]
        public async Task FindStreamMetadataAsync_ForNewStream_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var eventStore = new AzureEventHubEventStore(configuration, mockEventSerializer.Object, mockLogger.Object);
            var streamId = Guid.NewGuid();

            // Act
            var result = await eventStore.FindStreamMetadataAsync(streamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task SetStreamMetadata_ShouldStoreMetadata()
        {
            // Arrange
            var eventStore = new AzureEventHubEventStore(configuration, mockEventSerializer.Object, mockLogger.Object);
            var streamId = Guid.NewGuid();
            var metadata = new Dictionary<string, string> { ["key1"] = "value1", ["key2"] = "value2" };

            // Act
            eventStore.SetStreamMetadata(streamId, metadata);
            var result = await eventStore.FindStreamMetadataAsync(streamId);

            // Assert
            result.Should().BeEquivalentTo(metadata);
        }

        [Fact]
        public async Task BatchFindStreamMetadataAsync_ShouldReturnMetadataForMultipleStreams()
        {
            // Arrange
            var eventStore = new AzureEventHubEventStore(configuration, mockEventSerializer.Object, mockLogger.Object);
            var streamId1 = Guid.NewGuid();
            var streamId2 = Guid.NewGuid();
            var metadata1 = new Dictionary<string, string> { ["key1"] = "value1" };
            var metadata2 = new Dictionary<string, string> { ["key2"] = "value2" };

            // Act
            eventStore.SetStreamMetadata(streamId1, metadata1);
            eventStore.SetStreamMetadata(streamId2, metadata2);
            var result = await eventStore.BatchFindStreamMetadataAsync(new[] { streamId1, streamId2 });

            // Assert
            result.Should().HaveCount(2);
            result[streamId1].Should().BeEquivalentTo(metadata1);
            result[streamId2].Should().BeEquivalentTo(metadata2);
        }

        [Fact]
        public async Task GetEventsAsync_ShouldReturnEmptyCollection()
        {
            // Arrange
            var eventStore = new AzureEventHubEventStore(configuration, mockEventSerializer.Object, mockLogger.Object);
            var streamId = Guid.NewGuid();

            // Act
            var result = await eventStore.GetEventsAsync(streamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task FindEventsAsync_ShouldReturnEmptyCollection()
        {
            // Arrange
            var eventStore = new AzureEventHubEventStore(configuration, mockEventSerializer.Object, mockLogger.Object);
            var streamId = Guid.NewGuid();

            // Act
            var result = await eventStore.FindEventsAsync(streamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetEventRangeAsync_ShouldReturnEmptyCollection()
        {
            // Arrange
            var eventStore = new AzureEventHubEventStore(configuration, mockEventSerializer.Object, mockLogger.Object);
            var streamId = Guid.NewGuid();

            // Act
            var result = await eventStore.GetEventRangeAsync(streamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllEventsBackwardsAsync_ShouldThrowNotSupportedException()
        {
            // Arrange
            var eventStore = new AzureEventHubEventStore(configuration, mockEventSerializer.Object, mockLogger.Object);

            // Act & Assert
            var action = async () => await eventStore.GetAllEventsBackwardsAsync();
            await action.Should().ThrowAsync<NotSupportedException>();
        }

        [Fact]
        public async Task GetEventAsync_ShouldThrowNotSupportedException()
        {
            // Arrange
            var eventStore = new AzureEventHubEventStore(configuration, mockEventSerializer.Object, mockLogger.Object);
            var streamId = Guid.NewGuid();

            // Act & Assert
            var action = async () => await eventStore.GetEventAsync(streamId, 1);
            await action.Should().ThrowAsync<NotSupportedException>();
        }

        [Fact]
        public async Task BatchFindEventsAsync_ShouldThrowNotSupportedException()
        {
            // Arrange
            var eventStore = new AzureEventHubEventStore(configuration, mockEventSerializer.Object, mockLogger.Object);
            var streamIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

            // Act & Assert
            var action = async () => await eventStore.BatchFindEventsAsync(streamIds);
            await action.Should().ThrowAsync<NotSupportedException>();
        }
    }
}
