using System;
using System.Collections.Generic;
using Revo.Core.Events;
using Revo.Infrastructure.EventStores;

namespace Revo.AzureEventHub.EventStores
{
    /// <summary>
    /// Event store record implementation for Azure Event Hub.
    /// </summary>
    public class AzureEventHubEventStoreRecord : IEventStoreRecord
    {
        private readonly IEvent @event;
        private readonly IReadOnlyDictionary<string, string> additionalMetadata;

        public AzureEventHubEventStoreRecord(
            Guid eventId,
            IEvent @event,
            long streamSequenceNumber,
            DateTimeOffset storeDate,
            IReadOnlyDictionary<string, string> additionalMetadata)
        {
            EventId = eventId;
            this.@event = @event;
            StreamSequenceNumber = streamSequenceNumber;
            StoreDate = storeDate;
            this.additionalMetadata = additionalMetadata ?? new Dictionary<string, string>();
        }

        public Guid EventId { get; }
        public IEvent Event => @event;
        public long StreamSequenceNumber { get; }
        public DateTimeOffset StoreDate { get; }
        public IReadOnlyDictionary<string, string> AdditionalMetadata => additionalMetadata;
    }
}
