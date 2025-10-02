using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Revo.DependencyInjection.Core;

namespace Revo.DependencyInjection.Ninject
{
    /// <summary>
    /// Ninject implementation of the service container.
    /// </summary>
    public class NinjectServiceContainer : IServiceContainer
    {
        private readonly IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectServiceContainer"/> class.
        /// </summary>
        /// <param name="kernel">The Ninject kernel.</param>
        public NinjectServiceContainer(IKernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }

        /// <summary>
        /// Gets the underlying Ninject kernel.
        /// </summary>
        public IKernel Kernel => _kernel;

        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>The resolved service instance.</returns>
        public T GetService<T>()
        {
            return _kernel.Get<T>();
        }

        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <param name="serviceType">The type of service to resolve.</param>
        /// <returns>The resolved service instance.</returns>
        public object GetService(Type serviceType)
        {
            return _kernel.Get(serviceType);
        }

        /// <summary>
        /// Gets all services of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of services to resolve.</typeparam>
        /// <returns>All resolved service instances.</returns>
        public IEnumerable<T> GetServices<T>()
        {
            return _kernel.GetAll<T>();
        }

        /// <summary>
        /// Gets all services of the specified type.
        /// </summary>
        /// <param name="serviceType">The type of services to resolve.</param>
        /// <returns>All resolved service instances.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        /// <summary>
        /// Creates a new service scope.
        /// </summary>
        /// <returns>A new service scope.</returns>
        public IServiceScope CreateScope()
        {
            return new NinjectServiceScope(this);
        }

        /// <summary>
        /// Gets a service of the specified type, returning null if not registered.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>The resolved service instance or null if not registered.</returns>
        public T GetServiceOrDefault<T>()
        {
            try
            {
                return _kernel.Get<T>();
            }
            catch (ActivationException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Gets a service of the specified type, returning null if not registered.
        /// </summary>
        /// <param name="serviceType">The type of service to resolve.</param>
        /// <returns>The resolved service instance or null if not registered.</returns>
        public object GetServiceOrDefault(Type serviceType)
        {
            try
            {
                return _kernel.Get(serviceType);
            }
            catch (ActivationException)
            {
                return null;
            }
        }
    }
}
