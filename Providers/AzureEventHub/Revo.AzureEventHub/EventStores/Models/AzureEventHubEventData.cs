using System;
using System.Collections.Generic;
using System.Text.Json;
using Revo.Core.Events;

namespace Revo.AzureEventHub.EventStores.Models
{
    /// <summary>
    /// Represents event data stored in Azure Event Hub.
    /// </summary>
    public class AzureEventHubEventData
    {
        public Guid EventId { get; set; }
        public Guid StreamId { get; set; }
        public long StreamSequenceNumber { get; set; }
        public DateTimeOffset StoreDate { get; set; }
        public string EventName { get; set; } = string.Empty;
        public int EventVersion { get; set; }
        public string EventJson { get; set; } = string.Empty;
        public Dictionary<string, string> AdditionalMetadata { get; set; } = new();
        public Dictionary<string, string> EventHubProperties { get; set; } = new();

        /// <summary>
        /// Creates Azure Event Hub event data from a Revo event.
        /// </summary>
        public static AzureEventHubEventData FromEvent(
            IEvent @event,
            Guid eventId,
            Guid streamId,
            long streamSequenceNumber,
            DateTimeOffset storeDate,
            IReadOnlyDictionary<string, string> additionalMetadata,
            JsonSerializerOptions? jsonOptions = null)
        {
            var eventType = @event.GetType();
            var eventName = eventType.Name;
            var eventVersion = 1; // Could be extracted from version attributes if needed

            var eventJson = JsonSerializer.Serialize(@event, eventType, jsonOptions);

            return new AzureEventHubEventData
            {
                EventId = eventId,
                StreamId = streamId,
                StreamSequenceNumber = streamSequenceNumber,
                StoreDate = storeDate,
                EventName = eventName,
                EventVersion = eventVersion,
                EventJson = eventJson,
                AdditionalMetadata = new Dictionary<string, string>(additionalMetadata),
                EventHubProperties = new Dictionary<string, string>
                {
                    ["StreamId"] = streamId.ToString(),
                    ["StreamSequenceNumber"] = streamSequenceNumber.ToString(),
                    ["EventId"] = eventId.ToString(),
                    ["EventName"] = eventName,
                    ["EventVersion"] = eventVersion.ToString()
                }
            };
        }

        /// <summary>
        /// Converts to Azure Event Hub EventData.
        /// </summary>
        public Azure.Messaging.EventHubs.EventData ToEventData()
        {
            var eventData = new Azure.Messaging.EventHubs.EventData(JsonSerializer.SerializeToUtf8Bytes(this));
            
            foreach (var property in EventHubProperties)
            {
                eventData.Properties[property.Key] = property.Value;
            }

            return eventData;
        }

        /// <summary>
        /// Creates from Azure Event Hub EventData.
        /// </summary>
        public static AzureEventHubEventData FromEventData(Azure.Messaging.EventHubs.EventData eventData, JsonSerializerOptions? jsonOptions = null)
        {
            var json = System.Text.Encoding.UTF8.GetString(eventData.EventBody.ToArray());
            var data = JsonSerializer.Deserialize<AzureEventHubEventData>(json, jsonOptions) 
                ?? throw new InvalidOperationException("Failed to deserialize event data");

            // Copy properties from EventData
            foreach (var property in eventData.Properties)
            {
                data.EventHubProperties[property.Key] = property.Value.ToString() ?? string.Empty;
            }

            return data;
        }
    }
}
