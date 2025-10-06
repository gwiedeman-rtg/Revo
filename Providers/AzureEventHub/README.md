# Revo Azure Event Hub Provider

This provider enables using Azure Event Hub as an event store for the Revo framework.

## Features

- **Event Storage**: Store events in Azure Event Hub with proper partitioning
- **Stream Management**: Support for event streams with metadata
- **Partitioning Strategies**: Multiple strategies for distributing events across partitions
- **External Events**: Support for non-stream events via external event store
- **Configuration**: Flexible configuration options for different scenarios

## Installation

Add the NuGet package reference to your project:

```xml
<PackageReference Include="Revo.AzureEventHub" Version="1.0.0" />
```

## Configuration

### Basic Configuration

```csharp
configuration
    .UseAzureEventHubEventStore(options =>
    {
        options.ConnectionString = "your-connection-string";
        options.EventHubName = "your-event-hub";
    });
```

### Advanced Configuration

```csharp
configuration
    .UseAzureEventHubEventStore(options =>
    {
        options.ConnectionString = "your-connection-string";
        options.EventHubName = "your-event-hub";
        options.ConsumerGroup = "custom-consumer-group";
        options.PartitionStrategy = PartitionStrategy.HashByStreamId;
        options.MaxBatchSize = 100;
        options.MaxWaitTime = TimeSpan.FromSeconds(30);
        options.EnableCapture = true;
        options.CaptureStorageConnectionString = "your-storage-connection-string";
        options.CaptureContainerName = "eventhub-capture";
        options.RetryPolicy = new RetryPolicy
        {
            MaxRetries = 3,
            BaseDelay = TimeSpan.FromSeconds(1),
            MaxDelay = TimeSpan.FromMinutes(1),
            UseExponentialBackoff = true
        };
    });
```

## Partition Strategies

### HashByStreamId (Default)
Events are distributed across partitions based on the hash of the stream ID. This ensures that all events for a single stream are stored in the same partition, maintaining ordering.

### RoundRobin
Events are distributed across partitions in a round-robin fashion. This provides better load distribution but doesn't guarantee ordering within a stream.

### FixedPartition
All events are sent to a specific partition (partition 0). This is useful for testing or when you have a single partition.

## Limitations

### Read Operations
Azure Event Hub is primarily designed for high-throughput event ingestion. Complex read operations are not efficiently supported:

- `GetEventsAsync()` - Returns empty collection (not efficiently supported)
- `GetEventRangeAsync()` - Returns empty collection (not efficiently supported)
- `GetAllEventsBackwardsAsync()` - Throws `NotSupportedException`
- `GetEventAsync()` - Throws `NotSupportedException`
- `BatchFindEventsAsync()` - Throws `NotSupportedException`

### Recommended Solutions

For applications requiring complex event queries, consider:

1. **Event Hubs Capture**: Enable capture to automatically store events in Azure Blob Storage or Azure Data Lake Storage
2. **Separate Read Store**: Use a traditional database (SQL Server, PostgreSQL) for read operations
3. **Event Sourcing with Projections**: Use Revo's projection system to maintain read models

## Event Hubs Capture

When enabled, Event Hubs Capture automatically stores events in Azure Storage:

```csharp
options.EnableCapture = true;
options.CaptureStorageConnectionString = "your-storage-connection-string";
options.CaptureContainerName = "eventhub-capture";
```

This allows for:
- Long-term event retention
- Complex queries using Azure Storage tools
- Integration with Azure Data Factory and other analytics tools

## Performance Considerations

- **Batch Size**: Adjust `MaxBatchSize` based on your event size and throughput requirements
- **Wait Time**: Set `MaxWaitTime` to balance latency and throughput
- **Partitioning**: Choose the right partition strategy for your use case
- **Retry Policy**: Configure retry behavior for transient failures

## Monitoring

The provider integrates with Microsoft.Extensions.Logging for monitoring:

- Event store operations are logged at Debug level
- Warnings are logged for unsupported operations
- Errors are logged for failed operations

## Example Usage

```csharp
public class MyAggregate : EventSourcedAggregateRoot
{
    public void DoSomething()
    {
        // This will be stored in Azure Event Hub
        Publish(new SomethingHappenedEvent("data"));
    }
}

// In your application startup
var configuration = new RevoConfiguration();
configuration
    .UseAzureEventHubEventStore(options =>
    {
        options.ConnectionString = "your-connection-string";
        options.EventHubName = "your-event-hub";
    });
```

## Dependencies

- Azure.Messaging.EventHubs (5.11.4+)
- Azure.Messaging.EventHubs.Processor (5.11.4+)
- System.Text.Json (8.0.4+)
- Microsoft.Extensions.Logging.Abstractions (8.0.1+)

## License

This provider is part of the Revo framework and follows the same license terms.
