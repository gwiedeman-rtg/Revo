using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Revo.Core.Configuration;
using MSConfig = Microsoft.Extensions.Configuration;

namespace Revo.Core.Core
{
    /// <summary>
    /// Simplified core module using Microsoft DI directly.
    /// This replaces the complex CoreDependencyModule.
    /// </summary>
    public class CoreRevoModule : IRevoModule
    {
        public void Configure(IServiceCollection services, MSConfig.IConfiguration configuration, ILogger logger)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            // Register core services directly with Microsoft DI
            services.AddSingleton<IServiceLocator, MicrosoftServiceLocator>();
            
            // Register any other core services here
            // This is much simpler than the complex IServiceRegistry system
            
            logger.LogInformation("Core Revo module configured with Microsoft DI");
        }
    }
}
