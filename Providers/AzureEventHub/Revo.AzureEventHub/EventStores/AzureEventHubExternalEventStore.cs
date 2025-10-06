using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Logging;
using Revo.Core.Events;
using Revo.Infrastructure.Events.Async.Generic;
using Revo.Infrastructure.EventStores;
using Revo.AzureEventHub.Configuration;

namespace Revo.AzureEventHub.EventStores
{
    /// <summary>
    /// External event store implementation using Azure Event Hub.
    /// </summary>
    public class AzureEventHubExternalEventStore : IExternalEventStore
    {
        private readonly AzureEventHubEventStoreConfiguration configuration;
        private readonly IEventSerializer eventSerializer;
        private readonly ILogger<AzureEventHubExternalEventStore> logger;
        private readonly EventHubProducerClient producerClient;
        private readonly List<ExternalEventRecord> pendingEvents = new();

        public AzureEventHubExternalEventStore(
            AzureEventHubEventStoreConfiguration configuration,
            IEventSerializer eventSerializer,
            ILogger<AzureEventHubExternalEventStore> logger)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            producerClient = new EventHubProducerClient(configuration.ConnectionString, configuration.EventHubName);
        }

        public async Task<ExternalEventRecord> GetEventAsync(Guid eventId)
        {
            // Azure Event Hub doesn't support efficient lookup by event ID
            // This would require reading all events and filtering
            throw new NotSupportedException("GetEventAsync is not efficiently supported by Azure Event Hub. Consider using Event Hubs Capture for complex queries.");
        }

        public void TryPushEvent(IEventMessage eventMessage)
        {
            var eventRecord = new ExternalEventRecord(
                Guid.NewGuid(),
                eventMessage.Event,
                eventMessage.Metadata);

            pendingEvents.Add(eventRecord);
            logger.LogDebug("Added external event {EventId} to pending events", eventRecord.Id);
        }

        public async Task<ExternalEventRecord[]> CommitAsync()
        {
            if (!pendingEvents.Any())
                return Array.Empty<ExternalEventRecord>();

            var eventBatch = producerClient.CreateBatch();
            var committedEvents = new List<ExternalEventRecord>();

            foreach (var eventRecord in pendingEvents)
            {
                var eventData = new EventData(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(eventRecord));
                eventData.Properties["EventId"] = eventRecord.Id.ToString();
                eventData.Properties["EventName"] = eventRecord.Event.GetType().Name;
                eventData.Properties["IsExternalEvent"] = "true";

                if (!eventBatch.TryAdd(eventData))
                {
                    // Batch is full, send it and create a new one
                    await producerClient.SendAsync(eventBatch);
                    eventBatch = producerClient.CreateBatch();
                    
                    if (!eventBatch.TryAdd(eventData))
                    {
                        throw new InvalidOperationException("Event is too large for Event Hub batch");
                    }
                }

                committedEvents.Add(eventRecord);
            }

            // Send the final batch
            if (eventBatch.Count > 0)
            {
                await producerClient.SendAsync(eventBatch);
            }

            pendingEvents.Clear();
            logger.LogDebug("Committed {EventCount} external events to Azure Event Hub", committedEvents.Count);

            return committedEvents.ToArray();
        }

        public void Dispose()
        {
            producerClient?.Dispose();
        }
    }
}
