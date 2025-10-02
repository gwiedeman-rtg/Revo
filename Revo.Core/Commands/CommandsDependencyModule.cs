using System;
using Revo.Core.Commands;
using Revo.Core.Configuration;
using Revo.DependencyInjection.Core;
using Revo.DependencyInjection.Ninject;

namespace Revo.Core.Commands
{
    /// <summary>
    /// Commands dependency injection module using the new DI abstraction.
    /// This is the new version of CommandsModule that uses the abstracted dependency injection system.
    /// </summary>
    public class CommandsDependencyModule : NinjectDependencyModule
    {
        private readonly CommandsConfiguration commandsConfigurationSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandsDependencyModule"/> class.
        /// </summary>
        /// <param name="commandsConfigurationSection">The commands configuration section.</param>
        public CommandsDependencyModule(CommandsConfiguration commandsConfigurationSection)
        {
            this.commandsConfigurationSection = commandsConfigurationSection ?? throw new ArgumentNullException(nameof(commandsConfigurationSection));
        }

        /// <summary>
        /// Configures the services for this module.
        /// </summary>
        /// <param name="registry">The service registry to register services with.</param>
        /// <param name="context">The module configuration context.</param>
        public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
        {
            // Register command gateway
            registry.RegisterSingleton<ICommandGateway, CommandGateway>();
            
            // Register command bus
            registry.RegisterSingleton<ILocalCommandBus, LocalCommandBus>();
            
            // Register command bus middleware factory
            registry.RegisterSingleton<ICommandBusMiddlewareFactory, CommandBusMiddlewareFactory>();
            
            // Register command bus pipeline
            registry.RegisterTransient<ICommandBusPipeline, CommandBusPipeline>();
            
            // Register command context
            registry.RegisterScoped<ICommandContext, CommandContext>();
            
            // Register command bus middleware
            registry.RegisterScoped<ICommandBusMiddleware<ICommandBase>, FilterCommandBusMiddleware<ICommandBase>>();
        }
    }
}
