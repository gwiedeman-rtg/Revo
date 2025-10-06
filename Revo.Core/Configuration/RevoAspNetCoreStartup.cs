using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Revo.Core.Core;
using MSConfig = Microsoft.Extensions.Configuration;

namespace Revo.Core.Configuration
{
    /// <summary>
    /// Simplified ASP.NET Core startup using Microsoft DI.
    /// This eliminates the complex RevoStartup and uses standard .NET patterns.
    /// </summary>
    public abstract class RevoAspNetCoreStartup
    {
        protected MSConfig.IConfiguration Configuration { get; }
        protected ILogger Logger { get; }

        protected RevoAspNetCoreStartup(MSConfig.IConfiguration configuration, ILogger logger)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Configures services for the application.
        /// Override this method to add custom services.
        /// </summary>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            // Configure Revo using Microsoft DI
            var revoStartup = new RevoMicrosoftDIStartup(services, Configuration, Logger);
            revoStartup.Configure();

            // Configure additional services
            ConfigureAdditionalServices(services);
        }

        /// <summary>
        /// Configures additional services.
        /// Override this method to add custom services.
        /// </summary>
        protected virtual void ConfigureAdditionalServices(IServiceCollection services)
        {
            // Override in derived classes to add custom services
        }
    }
}
