using Revo.Infrastructure.EventStores;

namespace Revo.AzureEventHub.EventStores
{
    /// <summary>
    /// Event store implementation using Azure Event Hub as the backing store.
    /// </summary>
    public interface IAzureEventHubEventStore : IEventStore
    {
        /// <summary>
        /// Gets the name of the event source for this event store.
        /// </summary>
        string EventSourceName { get; }
    }
}
