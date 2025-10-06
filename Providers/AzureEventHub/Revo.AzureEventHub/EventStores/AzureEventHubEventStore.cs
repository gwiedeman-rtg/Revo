using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Logging;
using Revo.Core.Core;
using Revo.Core.Events;
using Revo.Core.Types;
using Revo.Domain.Events;
using Revo.Infrastructure.Events;
using Revo.Infrastructure.EventStores;
using Revo.AzureEventHub.Configuration;
using Revo.AzureEventHub.EventStores.Models;

namespace Revo.AzureEventHub.EventStores
{
    /// <summary>
    /// Event store implementation using Azure Event Hub as the backing store.
    /// </summary>
    public class AzureEventHubEventStore : IAzureEventHubEventStore
    {
        private readonly AzureEventHubEventStoreConfiguration configuration;
        private readonly IEventSerializer eventSerializer;
        private readonly ILogger<AzureEventHubEventStore> logger;
        private readonly JsonSerializerOptions jsonOptions;
        private readonly EventHubProducerClient producerClient;
        private readonly Dictionary<Guid, StreamMetadata> streamMetadata = new();
        private readonly SemaphoreSlim metadataLock = new(1, 1);

        public AzureEventHubEventStore(
            AzureEventHubEventStoreConfiguration configuration,
            IEventSerializer eventSerializer,
            ILogger<AzureEventHubEventStore> logger)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            configuration.Validate();

            jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            producerClient = new EventHubProducerClient(configuration.ConnectionString, configuration.EventHubName);
        }

        public string EventSourceName => "AzureEventHub.EventStore";

        public void AddStream(Guid streamId)
        {
            // Azure Event Hub doesn't require explicit stream creation
            // Streams are created implicitly when events are sent
            logger.LogDebug("Stream {StreamId} added to Azure Event Hub", streamId);
        }

        public async Task<IReadOnlyDictionary<Guid, IReadOnlyDictionary<string, string>>> BatchFindStreamMetadataAsync(Guid[] streamIds)
        {
            await metadataLock.WaitAsync();
            try
            {
                var result = new Dictionary<Guid, IReadOnlyDictionary<string, string>>();
                foreach (var streamId in streamIds)
                {
                    if (streamMetadata.TryGetValue(streamId, out var metadata))
                    {
                        result[streamId] = metadata.Metadata;
                    }
                    else
                    {
                        result[streamId] = new Dictionary<string, string>();
                    }
                }
                return result;
            }
            finally
            {
                metadataLock.Release();
            }
        }

        public async Task<IDictionary<Guid, IReadOnlyCollection<IEventStoreRecord>>> BatchFindEventsAsync(Guid[] streamIds)
        {
            // Azure Event Hub doesn't support efficient batch queries by stream ID
            // This would require reading all events and filtering, which is not efficient
            // In a real implementation, you might want to use Event Hubs Capture + Azure Storage
            // for complex queries, or maintain a separate index
            throw new NotSupportedException("Batch queries are not efficiently supported by Azure Event Hub. Consider using Event Hubs Capture with Azure Storage for complex queries.");
        }

        public async Task<IReadOnlyCollection<IEventStoreRecord>> FindEventsAsync(Guid streamId)
        {
            return await GetEventsAsync(streamId);
        }

        public async Task<IReadOnlyCollection<IEventStoreRecord>> GetEventsAsync(Guid streamId)
        {
            // This is a simplified implementation
            // In a real scenario, you'd need to read from Event Hub partitions
            // and filter by stream ID, which requires reading all events
            // For production use, consider using Event Hubs Capture + Azure Storage
            logger.LogWarning("GetEventsAsync is not efficiently implemented for Azure Event Hub. Consider using Event Hubs Capture for complex queries.");
            return new List<IEventStoreRecord>();
        }

        public async Task<IReadOnlyCollection<IEventStoreRecord>> GetEventRangeAsync(Guid streamId, long? minSequenceNumber = null, long? maxSequenceNumber = null, int? maxCount = null)
        {
            // Similar limitations as GetEventsAsync
            logger.LogWarning("GetEventRangeAsync is not efficiently implemented for Azure Event Hub. Consider using Event Hubs Capture for complex queries.");
            return new List<IEventStoreRecord>();
        }

        public async Task<EventStreamSlice> GetAllEventsBackwardsAsync(IStreamPosition position = null, int? maxCount = null)
        {
            // This would require reading from all partitions in reverse order
            // Not efficiently supported by Azure Event Hub
            throw new NotSupportedException("GetAllEventsBackwardsAsync is not efficiently supported by Azure Event Hub. Consider using Event Hubs Capture for complex queries.");
        }

        public async Task<IEventStoreRecord> GetEventAsync(Guid streamId, long sequenceNumber)
        {
            // This would require reading all events and filtering
            // Not efficiently supported by Azure Event Hub
            throw new NotSupportedException("GetEventAsync is not efficiently supported by Azure Event Hub. Consider using Event Hubs Capture for complex queries.");
        }

        public async Task<IReadOnlyDictionary<string, string>> FindStreamMetadataAsync(Guid streamId)
        {
            await metadataLock.WaitAsync();
            try
            {
                if (streamMetadata.TryGetValue(streamId, out var metadata))
                {
                    return metadata.Metadata;
                }
                return new Dictionary<string, string>();
            }
            finally
            {
                metadataLock.Release();
            }
        }

        public async Task<IReadOnlyDictionary<string, string>> GetStreamMetadataAsync(Guid streamId)
        {
            return await FindStreamMetadataAsync(streamId);
        }

        public async Task<EventStreamInfo> GetStreamInfoAsync(Guid streamId)
        {
            await metadataLock.WaitAsync();
            try
            {
                if (streamMetadata.TryGetValue(streamId, out var metadata))
                {
                    return new EventStreamInfo(streamId, metadata.EventCount, metadata.LastSequenceNumber);
                }
                return new EventStreamInfo(streamId, 0, 0);
            }
            finally
            {
                metadataLock.Release();
            }
        }

        public void SetStreamMetadata(Guid streamId, IReadOnlyDictionary<string, string> metadata)
        {
            metadataLock.Wait();
            try
            {
                if (!streamMetadata.TryGetValue(streamId, out var streamMeta))
                {
                    streamMeta = new StreamMetadata();
                    streamMetadata[streamId] = streamMeta;
                }
                streamMeta.Metadata = metadata.ToImmutableDictionary();
            }
            finally
            {
                metadataLock.Release();
            }
        }

        public async Task<IReadOnlyCollection<IEventStoreRecord>> PushEventsAsync(Guid streamId, IEnumerable<IUncommittedEventStoreRecord> events)
        {
            var eventList = events.ToList();
            if (!eventList.Any())
                return new List<IEventStoreRecord>();

            var committedEvents = new List<IEventStoreRecord>();
            var eventBatch = producerClient.CreateBatch();

            await metadataLock.WaitAsync();
            try
            {
                var streamMeta = GetOrCreateStreamMetadata(streamId);

                foreach (var uncommittedEvent in eventList)
                {
                    var eventId = Guid.NewGuid();
                    var sequenceNumber = streamMeta.NextSequenceNumber++;
                    var storeDate = Clock.Current.UtcNow;

                    var eventData = AzureEventHubEventData.FromEvent(
                        uncommittedEvent.Event,
                        eventId,
                        streamId,
                        sequenceNumber,
                        storeDate,
                        uncommittedEvent.Metadata,
                        jsonOptions);

                    var azureEventData = eventData.ToEventData();
                    
                    // Set partition key based on strategy
                    var partitionKey = GetPartitionKey(streamId);
                    if (!string.IsNullOrEmpty(partitionKey))
                    {
                        azureEventData.Properties["PartitionKey"] = partitionKey;
                    }

                    if (!eventBatch.TryAdd(azureEventData))
                    {
                        // Batch is full, send it and create a new one
                        await producerClient.SendAsync(eventBatch);
                        eventBatch = producerClient.CreateBatch();
                        
                        if (!eventBatch.TryAdd(azureEventData))
                        {
                            throw new InvalidOperationException("Event is too large for Event Hub batch");
                        }
                    }

                    var committedEvent = new AzureEventHubEventStoreRecord(
                        eventId,
                        uncommittedEvent.Event,
                        sequenceNumber,
                        storeDate,
                        uncommittedEvent.Metadata);

                    committedEvents.Add(committedEvent);
                }

                // Send the final batch
                if (eventBatch.Count > 0)
                {
                    await producerClient.SendAsync(eventBatch);
                }

                // Update stream metadata
                streamMeta.EventCount += eventList.Count;
                streamMeta.LastSequenceNumber = streamMeta.NextSequenceNumber - 1;
            }
            finally
            {
                metadataLock.Release();
            }

            logger.LogDebug("Pushed {EventCount} events to stream {StreamId}", eventList.Count, streamId);
            return committedEvents;
        }

        private StreamMetadata GetOrCreateStreamMetadata(Guid streamId)
        {
            if (!streamMetadata.TryGetValue(streamId, out var metadata))
            {
                metadata = new StreamMetadata();
                streamMetadata[streamId] = metadata;
            }
            return metadata;
        }

        private string? GetPartitionKey(Guid streamId)
        {
            return configuration.PartitionStrategy switch
            {
                PartitionStrategy.HashByStreamId => streamId.ToString(),
                PartitionStrategy.RoundRobin => null, // Let Event Hub distribute
                PartitionStrategy.FixedPartition => "0", // Use partition 0
                _ => streamId.ToString()
            };
        }

        public void Dispose()
        {
            producerClient?.Dispose();
            metadataLock?.Dispose();
        }

        private class StreamMetadata
        {
            public long NextSequenceNumber { get; set; } = 1;
            public long EventCount { get; set; } = 0;
            public long LastSequenceNumber { get; set; } = 0;
            public ImmutableDictionary<string, string> Metadata { get; set; } = ImmutableDictionary<string, string>.Empty;
        }
    }
}
