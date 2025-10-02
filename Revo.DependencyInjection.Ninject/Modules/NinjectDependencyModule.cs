using System;
using Revo.DependencyInjection.Core;

namespace Revo.DependencyInjection.Ninject
{
    /// <summary>
    /// Base class for Ninject dependency injection modules.
    /// Provides a convenient base class for modules that use the new DI abstraction.
    /// </summary>
    public abstract class NinjectDependencyModule : DependencyModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectDependencyModule"/> class.
        /// </summary>
        protected NinjectDependencyModule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectDependencyModule"/> class.
        /// </summary>
        /// <param name="name">The name of the module.</param>
        /// <param name="autoLoad">Whether the module should be automatically loaded.</param>
        protected NinjectDependencyModule(string name, bool autoLoad = true) : base(name, autoLoad)
        {
        }
    }
}
