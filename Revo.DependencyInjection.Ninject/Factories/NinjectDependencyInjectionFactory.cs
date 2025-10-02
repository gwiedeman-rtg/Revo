using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ninject;
using Ninject.Modules;
using Revo.DependencyInjection.Core;

namespace Revo.DependencyInjection.Ninject
{
    /// <summary>
    /// Factory for creating Ninject-based dependency injection components.
    /// </summary>
    public static class NinjectDependencyInjectionFactory
    {
        /// <summary>
        /// Creates a new Ninject service container.
        /// </summary>
        /// <param name="kernel">The Ninject kernel.</param>
        /// <returns>A new service container.</returns>
        public static IServiceContainer CreateServiceContainer(IKernel kernel)
        {
            return new NinjectServiceContainer(kernel);
        }

        /// <summary>
        /// Creates a new Ninject service registry.
        /// </summary>
        /// <param name="kernel">The Ninject kernel.</param>
        /// <returns>A new service registry.</returns>
        public static IServiceRegistry CreateServiceRegistry(IKernel kernel)
        {
            return new NinjectServiceRegistry(kernel);
        }

        /// <summary>
        /// Creates a new Ninject module loader.
        /// </summary>
        /// <param name="kernel">The Ninject kernel.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A new module loader.</returns>
        public static Revo.DependencyInjection.Core.IModuleLoader CreateModuleLoader(IKernel kernel, IConfiguration configuration, ILogger logger)
        {
            return new NinjectModuleLoader(kernel, configuration, logger);
        }

        /// <summary>
        /// Creates a new Ninject module configuration context.
        /// </summary>
        /// <param name="kernel">The Ninject kernel.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A new module configuration context.</returns>
        public static IModuleConfigurationContext CreateModuleConfigurationContext(IKernel kernel, IConfiguration configuration)
        {
            return new NinjectModuleConfigurationContext(kernel, configuration);
        }

        /// <summary>
        /// Creates a new Ninject kernel with default configuration.
        /// </summary>
        /// <returns>A new Ninject kernel.</returns>
        public static IKernel CreateKernel()
        {
            return new StandardKernel();
        }

        /// <summary>
        /// Creates a new Ninject kernel with the specified modules.
        /// </summary>
        /// <param name="modules">The modules to load.</param>
        /// <returns>A new Ninject kernel.</returns>
        public static IKernel CreateKernel(params INinjectModule[] modules)
        {
            return new StandardKernel(modules);
        }

        /// <summary>
        /// Creates a new Ninject kernel with the specified configuration.
        /// </summary>
        /// <param name="configuration">The kernel configuration.</param>
        /// <returns>A new Ninject kernel.</returns>
        public static IKernel CreateKernel(Action<IKernel> configuration)
        {
            var kernel = new StandardKernel();
            configuration?.Invoke(kernel);
            return kernel;
        }
    }
}
