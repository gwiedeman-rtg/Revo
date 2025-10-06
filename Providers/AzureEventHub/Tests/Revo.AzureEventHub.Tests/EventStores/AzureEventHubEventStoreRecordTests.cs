using System;
using System.Collections.Generic;
using FluentAssertions;
using Revo.Core.Events;
using Revo.AzureEventHub.EventStores;
using Xunit;

namespace Revo.AzureEventHub.Tests.EventStores
{
    public class AzureEventHubEventStoreRecordTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldSetProperties()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var @event = new TestEvent("Test Data");
            var streamSequenceNumber = 123L;
            var storeDate = DateTimeOffset.UtcNow;
            var additionalMetadata = new Dictionary<string, string> { ["key1"] = "value1" };

            // Act
            var record = new AzureEventHubEventStoreRecord(
                eventId,
                @event,
                streamSequenceNumber,
                storeDate,
                additionalMetadata);

            // Assert
            record.EventId.Should().Be(eventId);
            record.Event.Should().Be(@event);
            record.StreamSequenceNumber.Should().Be(streamSequenceNumber);
            record.StoreDate.Should().Be(storeDate);
            record.AdditionalMetadata.Should().BeEquivalentTo(additionalMetadata);
        }

        [Fact]
        public void Constructor_WithNullAdditionalMetadata_ShouldUseEmptyDictionary()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var @event = new TestEvent("Test Data");
            var streamSequenceNumber = 123L;
            var storeDate = DateTimeOffset.UtcNow;

            // Act
            var record = new AzureEventHubEventStoreRecord(
                eventId,
                @event,
                streamSequenceNumber,
                storeDate,
                null);

            // Assert
            record.AdditionalMetadata.Should().NotBeNull();
            record.AdditionalMetadata.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WithEmptyAdditionalMetadata_ShouldUseEmptyDictionary()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var @event = new TestEvent("Test Data");
            var streamSequenceNumber = 123L;
            var storeDate = DateTimeOffset.UtcNow;
            var additionalMetadata = new Dictionary<string, string>();

            // Act
            var record = new AzureEventHubEventStoreRecord(
                eventId,
                @event,
                streamSequenceNumber,
                storeDate,
                additionalMetadata);

            // Assert
            record.AdditionalMetadata.Should().NotBeNull();
            record.AdditionalMetadata.Should().BeEmpty();
        }

        private class TestEvent : IEvent
        {
            public string Data { get; }

            public TestEvent(string data)
            {
                Data = data;
            }
        }
    }
}
