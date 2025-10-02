using System;
using Revo.Core.Commands;
using Revo.Core.Configuration;
using Revo.Core.Events;
using Revo.Core.Lifecycle;
using Revo.Core.Security;
using Revo.Core.Transactions;
using Revo.Core.Types;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Ninject;
using Ninject.Extensions.ContextPreservation;

namespace Revo.Core.Core
{
    /// <summary>
    /// Core dependency injection module using the new DI abstraction.
    /// This is the new version of CoreModule that uses the abstracted dependency injection system.
    /// </summary>
    [AutoLoadModule(false)]
    public class CoreDependencyModule : NinjectDependencyModule
    {
        private readonly CoreConfigurationSection coreConfigurationSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreDependencyModule"/> class.
        /// </summary>
        /// <param name="coreConfigurationSection">The core configuration section.</param>
        public CoreDependencyModule(CoreConfigurationSection coreConfigurationSection)
        {
            this.coreConfigurationSection = coreConfigurationSection ?? throw new ArgumentNullException(nameof(coreConfigurationSection));
        }

        /// <summary>
        /// Configures the services for this module.
        /// </summary>
        /// <param name="registry">The service registry to register services with.</param>
        /// <param name="context">The module configuration context.</param>
        public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
        {
            // Configure Ninject-specific components if using Ninject registry
            if (registry is NinjectServiceRegistry ninjectRegistry)
            {
                ninjectRegistry.Kernel.Components.Add<Ninject.Planning.Bindings.Resolvers.IBindingResolver, ContravariantBindingResolver>();
            }

            // Register core services
            registry.RegisterFactory<IClock>(provider => Clock.Current, ServiceLifetime.Transient);

            registry.RegisterSingleton<IEnvironment, Environment>()
                .WithPropertyValue(nameof(Environment.IsDevelopmentOverride), coreConfigurationSection.IsDevelopmentEnvironment);

            registry.RegisterScoped<IEventBus, EventBus>();
            registry.RegisterScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

            // Register IUnitOfWork with context-preserving method
            if (registry is NinjectServiceRegistry ninjectReg)
            {
                ninjectReg.RegisterMethod<IUnitOfWork>(
                    ctx => ctx.ContextPreservingGet<ICommandContext>().UnitOfWork
                           ?? throw new InvalidOperationException("Trying to resolve IUnitOfWork when there is not one active in current command context"),
                    ServiceLifetime.Transient);
            }
            else
            {
                // Fallback for non-Ninject registries
                registry.RegisterFactory<IUnitOfWork>(provider => 
                    throw new NotSupportedException("IUnitOfWork resolution requires Ninject context preservation"), 
                    ServiceLifetime.Transient);
            }

            registry.RegisterScoped<IPublishEventBufferFactory, PublishEventBufferFactory>();

            // Register IPublishEventBuffer with context-preserving method
            if (registry is NinjectServiceRegistry ninjectReg2)
            {
                ninjectReg2.RegisterMethod<IPublishEventBuffer>(
                    ctx => ctx.ContextPreservingGet<IUnitOfWork>().EventBuffer,
                    ServiceLifetime.Transient);
            }
            else
            {
                // Fallback for non-Ninject registries
                registry.RegisterFactory<IPublishEventBuffer>(provider => 
                    throw new NotSupportedException("IPublishEventBuffer resolution requires Ninject context preservation"), 
                    ServiceLifetime.Transient);
            }

            // Rebind type services
            registry.Rebind<ITypeExplorer, TypeExplorer>(ServiceLifetime.Singleton);
            registry.Rebind<ITypeIndexer, TypeIndexer>(ServiceLifetime.Singleton);
            registry.Rebind<IVersionedTypeRegistry, VersionedTypeRegistry>(ServiceLifetime.Singleton);

            // Register security services
            registry.RegisterSingleton<IPermissionTypeRegistry, PermissionTypeRegistry>();
            registry.RegisterMultiple(
                new[] { typeof(IPermissionTypeIndexer), typeof(IApplicationStartedListener) },
                typeof(PermissionTypeIndexer),
                ServiceLifetime.Singleton);
            registry.RegisterScoped<IPermissionAuthorizationMatcher, PermissionAuthorizationMatcher>();
            registry.RegisterScoped<IUserPermissionAuthorizer, UserPermissionAuthorizer>();
        }
    }
}
