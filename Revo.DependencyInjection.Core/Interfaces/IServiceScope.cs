using System;

namespace Revo.DependencyInjection.Core
{
    /// <summary>
    /// Represents a scope for service resolution and lifetime management.
    /// </summary>
    public interface IServiceScope : IDisposable
    {
        /// <summary>
        /// Gets the service provider for this scope.
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets a service of the specified type from this scope.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>The resolved service instance.</returns>
        T GetService<T>();

        /// <summary>
        /// Gets a service of the specified type from this scope.
        /// </summary>
        /// <param name="serviceType">The type of service to resolve.</param>
        /// <returns>The resolved service instance.</returns>
        object GetService(Type serviceType);

        /// <summary>
        /// Gets all services of the specified type from this scope.
        /// </summary>
        /// <typeparam name="T">The type of services to resolve.</typeparam>
        /// <returns>All resolved service instances.</returns>
        System.Collections.Generic.IEnumerable<T> GetServices<T>();

        /// <summary>
        /// Gets all services of the specified type from this scope.
        /// </summary>
        /// <param name="serviceType">The type of services to resolve.</param>
        /// <returns>All resolved service instances.</returns>
        System.Collections.Generic.IEnumerable<object> GetServices(Type serviceType);
    }
}
