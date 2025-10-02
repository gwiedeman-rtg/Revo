using System;
using System.Collections.Generic;

namespace Revo.DependencyInjection.Core
{
    /// <summary>
    /// Provides a service container for resolving dependencies.
    /// This is the primary interface for service resolution in the Revo framework.
    /// </summary>
    public interface IServiceContainer
    {
        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>The resolved service instance.</returns>
        T GetService<T>();

        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <param name="serviceType">The type of service to resolve.</param>
        /// <returns>The resolved service instance.</returns>
        object GetService(Type serviceType);

        /// <summary>
        /// Gets all services of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of services to resolve.</typeparam>
        /// <returns>All resolved service instances.</returns>
        IEnumerable<T> GetServices<T>();

        /// <summary>
        /// Gets all services of the specified type.
        /// </summary>
        /// <param name="serviceType">The type of services to resolve.</param>
        /// <returns>All resolved service instances.</returns>
        IEnumerable<object> GetServices(Type serviceType);

        /// <summary>
        /// Creates a new service scope.
        /// </summary>
        /// <returns>A new service scope.</returns>
        IServiceScope CreateScope();

        /// <summary>
        /// Gets a service of the specified type, returning null if not registered.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>The resolved service instance or null if not registered.</returns>
        T GetServiceOrDefault<T>();

        /// <summary>
        /// Gets a service of the specified type, returning null if not registered.
        /// </summary>
        /// <param name="serviceType">The type of service to resolve.</param>
        /// <returns>The resolved service instance or null if not registered.</returns>
        object GetServiceOrDefault(Type serviceType);
    }
}
