using System;

namespace Revo.AzureEventHub.Configuration
{
    /// <summary>
    /// Configuration for Azure Event Hub event store.
    /// </summary>
    public class AzureEventHubEventStoreConfiguration
    {
        /// <summary>
        /// Azure Event Hub connection string.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Azure Event Hub name.
        /// </summary>
        public string EventHubName { get; set; } = string.Empty;

        /// <summary>
        /// Consumer group name for reading events.
        /// </summary>
        public string ConsumerGroup { get; set; } = "$Default";

        /// <summary>
        /// Partition strategy for distributing events across partitions.
        /// </summary>
        public PartitionStrategy PartitionStrategy { get; set; } = PartitionStrategy.HashByStreamId;

        /// <summary>
        /// Maximum number of events to retrieve in a single batch.
        /// </summary>
        public int MaxBatchSize { get; set; } = 100;

        /// <summary>
        /// Maximum wait time for batch operations.
        /// </summary>
        public TimeSpan MaxWaitTime { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Retry policy for transient failures.
        /// </summary>
        public RetryPolicy RetryPolicy { get; set; } = new();

        /// <summary>
        /// Whether to enable Event Hubs Capture for long-term storage.
        /// </summary>
        public bool EnableCapture { get; set; } = false;

        /// <summary>
        /// Storage account connection string for Event Hubs Capture.
        /// </summary>
        public string? CaptureStorageConnectionString { get; set; }

        /// <summary>
        /// Container name for Event Hubs Capture.
        /// </summary>
        public string CaptureContainerName { get; set; } = "eventhub-capture";

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new InvalidOperationException("ConnectionString is required");

            if (string.IsNullOrWhiteSpace(EventHubName))
                throw new InvalidOperationException("EventHubName is required");

            if (EnableCapture && string.IsNullOrWhiteSpace(CaptureStorageConnectionString))
                throw new InvalidOperationException("CaptureStorageConnectionString is required when EnableCapture is true");
        }
    }

    /// <summary>
    /// Partition strategy for distributing events across Event Hub partitions.
    /// </summary>
    public enum PartitionStrategy
    {
        /// <summary>
        /// Hash the stream ID to determine partition.
        /// </summary>
        HashByStreamId,

        /// <summary>
        /// Use round-robin distribution.
        /// </summary>
        RoundRobin,

        /// <summary>
        /// Use a specific partition.
        /// </summary>
        FixedPartition
    }

    /// <summary>
    /// Retry policy configuration.
    /// </summary>
    public class RetryPolicy
    {
        /// <summary>
        /// Maximum number of retry attempts.
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// Base delay between retries.
        /// </summary>
        public TimeSpan BaseDelay { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Maximum delay between retries.
        /// </summary>
        public TimeSpan MaxDelay { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Whether to use exponential backoff.
        /// </summary>
        public bool UseExponentialBackoff { get; set; } = true;
    }
}
