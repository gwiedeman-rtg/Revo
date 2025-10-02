using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Revo.DependencyInjection.Core;

namespace Revo.DependencyInjection.Microsoft
{
    /// <summary>
    /// Microsoft.Extensions.DependencyInjection implementation of the service container.
    /// </summary>
    public class MicrosoftServiceContainer : IServiceContainer
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftServiceContainer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The Microsoft service provider.</param>
        public MicrosoftServiceContainer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Gets the underlying Microsoft service provider.
        /// </summary>
        public IServiceProvider ServiceProvider => _serviceProvider;

        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>The resolved service instance.</returns>
        public T GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }

        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <param name="serviceType">The type of service to resolve.</param>
        /// <returns>The resolved service instance.</returns>
        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        /// <summary>
        /// Gets all services of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of services to resolve.</typeparam>
        /// <returns>All resolved service instances.</returns>
        public IEnumerable<T> GetServices<T>()
        {
            return _serviceProvider.GetServices<T>();
        }

        /// <summary>
        /// Gets all services of the specified type.
        /// </summary>
        /// <param name="serviceType">The type of services to resolve.</param>
        /// <returns>All resolved service instances.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _serviceProvider.GetServices(serviceType);
        }

        /// <summary>
        /// Creates a new service scope.
        /// </summary>
        /// <returns>A new service scope.</returns>
        public Revo.DependencyInjection.Core.IServiceScope CreateScope()
        {
            return new MicrosoftServiceScope(this);
        }

        /// <summary>
        /// Gets a service of the specified type, returning null if not registered.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>The resolved service instance or null if not registered.</returns>
        public T GetServiceOrDefault<T>()
        {
            return _serviceProvider.GetService<T>();
        }

        /// <summary>
        /// Gets a service of the specified type, returning null if not registered.
        /// </summary>
        /// <param name="serviceType">The type of service to resolve.</param>
        /// <returns>The resolved service instance or null if not registered.</returns>
        public object GetServiceOrDefault(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
    }
}
