using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Revo.Core.Configuration;
using Revo.Domain.Entities.EventSourcing;
using Revo.Domain.Events;
using Revo.AzureEventHub.Configuration;

namespace Revo.Examples.AzureEventHub
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            using var scope = host.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            
            try
            {
                logger.LogInformation("Starting Azure Event Hub example...");
                
                // This is a simplified example - in a real application,
                // you would use the Revo framework's dependency injection
                // and event sourcing infrastructure
                
                logger.LogInformation("Azure Event Hub example completed successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during execution");
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Configure Revo with Azure Event Hub
                    var configuration = new RevoConfiguration();
                    configuration
                        .UseAzureEventHubEventStore(options =>
                        {
                            // In a real application, these would come from configuration
                            options.ConnectionString = "your-connection-string";
                            options.EventHubName = "your-event-hub";
                            options.PartitionStrategy = PartitionStrategy.HashByStreamId;
                        });
                    
                    // Register services
                    services.AddSingleton(configuration);
                });
    }

    // Example aggregate for demonstration
    public class ExampleAggregate : EventSourcedAggregateRoot
    {
        public string Name { get; private set; } = string.Empty;

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            Publish(new NameSetEvent(name));
        }

        private void Apply(NameSetEvent evt)
        {
            Name = evt.Name;
        }
    }

    // Example event
    public class NameSetEvent : IEvent
    {
        public string Name { get; }

        public NameSetEvent(string name)
        {
            Name = name;
        }
    }
}
