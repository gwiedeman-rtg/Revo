using System;
using Revo.AspNetCore.Core;
using Revo.Core.Core;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Ninject;
using Revo.Hangfire;

namespace Revo.AspNetCore
{
    /// <summary>
    /// ASP.NET Core dependency injection module using the new DI abstraction.
    /// This is the new version of AspNetCoreModule that uses the abstracted dependency injection system.
    /// </summary>
    [Revo.Core.Core.AutoLoadModule(false)]
    public class AspNetCoreDependencyModule : NinjectDependencyModule
    {
        private readonly HangfireConfigurationSection hangfireConfigurationSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetCoreDependencyModule"/> class.
        /// </summary>
        /// <param name="hangfireConfigurationSection">The Hangfire configuration section.</param>
        public AspNetCoreDependencyModule(HangfireConfigurationSection hangfireConfigurationSection)
        {
            this.hangfireConfigurationSection = hangfireConfigurationSection ?? throw new ArgumentNullException(nameof(hangfireConfigurationSection));
        }

        /// <summary>
        /// Configures the services for this module.
        /// </summary>
        /// <param name="registry">The service registry to register services with.</param>
        /// <param name="context">The module configuration context.</param>
        public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
        {
            // Register configuration
            registry.RegisterFactory<IConfiguration>(provider => LocalConfiguration.Current, ServiceLifetime.Transient);

            // Register actor context
            registry.RegisterScoped<IActorContext, UserActorContext>();

            // Register service locator
            registry.RegisterSingleton<IServiceLocator, NinjectServiceLocator>();

            // Register environment provider
            registry.RegisterSingleton<IEnvironmentProvider, AspNetCoreEnvironmentProvider>();

            // Register Hangfire startup configurer if active
            if (hangfireConfigurationSection.IsActive)
            {
                registry.RegisterSingleton<IAspNetCoreStartupConfigurer, HangfireStartupConfigurator>();
            }
        }
    }
}
