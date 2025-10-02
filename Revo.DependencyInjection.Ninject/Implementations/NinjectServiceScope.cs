using System;
using System.Collections.Generic;
using System.Linq;
using Revo.DependencyInjection.Core;

namespace Revo.DependencyInjection.Ninject
{
    /// <summary>
    /// Ninject implementation of the service scope.
    /// </summary>
    public class NinjectServiceScope : IServiceScope
    {
        private readonly NinjectServiceContainer _container;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectServiceScope"/> class.
        /// </summary>
        /// <param name="container">The Ninject service container.</param>
        public NinjectServiceScope(NinjectServiceContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <summary>
        /// Gets the service provider for this scope.
        /// </summary>
        public IServiceProvider ServiceProvider => (IServiceProvider)_container;

        /// <summary>
        /// Gets a service of the specified type from this scope.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>The resolved service instance.</returns>
        public T GetService<T>()
        {
            ThrowIfDisposed();
            return _container.GetService<T>();
        }

        /// <summary>
        /// Gets a service of the specified type from this scope.
        /// </summary>
        /// <param name="serviceType">The type of service to resolve.</param>
        /// <returns>The resolved service instance.</returns>
        public object GetService(Type serviceType)
        {
            ThrowIfDisposed();
            return _container.GetService(serviceType);
        }

        /// <summary>
        /// Gets all services of the specified type from this scope.
        /// </summary>
        /// <typeparam name="T">The type of services to resolve.</typeparam>
        /// <returns>All resolved service instances.</returns>
        public IEnumerable<T> GetServices<T>()
        {
            ThrowIfDisposed();
            return _container.GetServices<T>();
        }

        /// <summary>
        /// Gets all services of the specified type from this scope.
        /// </summary>
        /// <param name="serviceType">The type of services to resolve.</param>
        /// <returns>All resolved service instances.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            ThrowIfDisposed();
            return _container.GetServices(serviceType);
        }

        /// <summary>
        /// Disposes the scope and any scoped services.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the scope and any scoped services.
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Ninject handles scope disposal automatically
                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(NinjectServiceScope));
            }
        }
    }
}
