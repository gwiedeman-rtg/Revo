using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Revo.Infrastructure.DataAccess.Migrations;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Microsoft;

namespace Revo.Examples.Todos.MicrosoftDI
{
    public class Module : MicrosoftDependencyModule
    {
        public override void Configure(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            services.AddSingleton<ResourceDatabaseMigrationDiscoveryAssembly>(
                provider => new ResourceDatabaseMigrationDiscoveryAssembly(GetType().Assembly, "Sql"));
        }
    }
}