using Revo.Core.Security;
using Revo.Core.Security.ClaimBased;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Ninject;

namespace Revo.Core.Security
{
    /// <summary>
    /// Null core security dependency injection module using the new DI abstraction.
    /// This is the new version of NullCoreSecurityModule that uses the abstracted dependency injection system.
    /// </summary>
    [AutoLoadModule(false)]
    public class NullCoreSecurityDependencyModule : NinjectDependencyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullCoreSecurityDependencyModule"/> class.
        /// </summary>
        public NullCoreSecurityDependencyModule()
        {
        }

        /// <summary>
        /// Configures the services for this module.
        /// </summary>
        /// <param name="registry">The service registry to register services with.</param>
        /// <param name="context">The module configuration context.</param>
        public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
        {
            // Register null implementations for security services
            registry.RegisterSingleton<IClaimsPrincipalUserResolver, NullClaimsPrincipalUserResolver>();
            registry.RegisterSingleton<IUserPermissionResolver, NullUserPermissionResolver>();
        }
    }
}
