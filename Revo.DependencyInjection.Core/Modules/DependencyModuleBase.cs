using System;
using System.Reflection;

namespace Revo.DependencyInjection.Core
{
    /// <summary>
    /// Base class for dependency injection modules.
    /// Provides common functionality and default implementations.
    /// </summary>
    public abstract class DependencyModuleBase : IDependencyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyModuleBase"/> class.
        /// </summary>
        protected DependencyModuleBase()
        {
            Name = GetType().Name;
            AutoLoad = GetAutoLoadFromAttribute();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyModuleBase"/> class.
        /// </summary>
        /// <param name="name">The name of the module.</param>
        /// <param name="autoLoad">Whether the module should be automatically loaded.</param>
        protected DependencyModuleBase(string name, bool autoLoad = true)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AutoLoad = autoLoad;
        }

        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this module should be automatically loaded.
        /// </summary>
        public bool AutoLoad { get; }

        /// <summary>
        /// Configures the services for this module.
        /// </summary>
        /// <param name="registry">The service registry to register services with.</param>
        /// <param name="context">The module configuration context.</param>
        public abstract void Configure(IServiceRegistry registry, IModuleConfigurationContext context);

        private bool GetAutoLoadFromAttribute()
        {
            var attribute = GetType().GetCustomAttribute<AutoLoadModuleAttribute>();
            return attribute?.AutoLoad ?? true;
        }
    }
}
