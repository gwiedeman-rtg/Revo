using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Revo.Core.Configuration;
using Revo.Infrastructure.DataAccess.Migrations;
using MSConfig = Microsoft.Extensions.Configuration;

namespace Revo.Examples.Todos.MicrosoftDI
{
    /// <summary>
    /// Simple todo module using Microsoft DI directly.
    /// This replaces the complex Ninject module system.
    /// </summary>
    public class TodoRevoModule : IRevoModule
    {
        public void Configure(IServiceCollection services, MSConfig.IConfiguration configuration, ILogger logger)
        {
            // Register todo-specific services directly with Microsoft DI
            services.AddSingleton<ResourceDatabaseMigrationDiscoveryAssembly>(
                provider => new ResourceDatabaseMigrationDiscoveryAssembly(GetType().Assembly, "Sql"));
            
            // Register any other todo services here
            // This is much simpler than the complex IServiceRegistry system
            
            logger.LogInformation("Todo module configured with Microsoft DI");
        }
    }
}
