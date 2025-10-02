using Revo.Core.Lifecycle;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Ninject;

namespace Revo.Infrastructure.DataAccess
{
    /// <summary>
    /// Data access dependency injection module using the new DI abstraction.
    /// This is the new version of DataAccessModule that uses the abstracted dependency injection system.
    /// </summary>
    public class DataAccessDependencyModule : NinjectDependencyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessDependencyModule"/> class.
        /// </summary>
        public DataAccessDependencyModule()
        {
        }

        /// <summary>
        /// Configures the services for this module.
        /// </summary>
        /// <param name="registry">The service registry to register services with.</param>
        /// <param name="context">The module configuration context.</param>
        public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
        {
            // Register database initializer services
            registry.RegisterMultiple(
                new[] { typeof(IDatabaseInitializerLoader), typeof(IApplicationStartedListener) },
                typeof(DatabaseInitializerLoader),
                ServiceLifetime.Singleton);

            registry.RegisterSingleton<IDatabaseInitializerDiscovery, DatabaseInitializerDiscovery>();
            registry.RegisterSingleton<IDatabaseInitializerSorter, DatabaseInitializerDependencySorter>();
        }
    }
}
