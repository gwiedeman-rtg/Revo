using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Revo.DependencyInjection.Core;
using MSDI = Microsoft.Extensions.DependencyInjection;

namespace Revo.DependencyInjection.Microsoft
{
    /// <summary>
    /// Microsoft.Extensions.DependencyInjection implementation of the service scope.
    /// </summary>
    public class MicrosoftServiceScope : Revo.DependencyInjection.Core.IServiceScope
    {
        private readonly MicrosoftServiceContainer _container;
        private readonly MSDI.IServiceScope _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftServiceScope"/> class.
        /// </summary>
        /// <param name="container">The parent service container.</param>
        public MicrosoftServiceScope(MicrosoftServiceContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _scope = container.ServiceProvider.CreateScope();
        }

        /// <summary>
        /// Gets the service provider for this scope.
        /// </summary>
        public IServiceProvider ServiceProvider => _scope.ServiceProvider;

        /// <summary>
        /// Gets a service of the specified type from this scope.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>The resolved service instance.</returns>
        public T GetService<T>()
        {
            return _scope.ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// Gets a service of the specified type from this scope.
        /// </summary>
        /// <param name="serviceType">The type of service to resolve.</param>
        /// <returns>The resolved service instance.</returns>
        public object GetService(Type serviceType)
        {
            return _scope.ServiceProvider.GetService(serviceType);
        }

        /// <summary>
        /// Gets all services of the specified type from this scope.
        /// </summary>
        /// <typeparam name="T">The type of services to resolve.</typeparam>
        /// <returns>All resolved service instances.</returns>
        public IEnumerable<T> GetServices<T>()
        {
            return _scope.ServiceProvider.GetServices<T>();
        }

        /// <summary>
        /// Gets all services of the specified type from this scope.
        /// </summary>
        /// <param name="serviceType">The type of services to resolve.</param>
        /// <returns>All resolved service instances.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _scope.ServiceProvider.GetServices(serviceType);
        }

        /// <summary>
        /// Disposes the service scope.
        /// </summary>
        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
