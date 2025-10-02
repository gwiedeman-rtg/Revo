using System;
using System.Collections.Generic;

namespace Revo.DependencyInjection.Core
{
    /// <summary>
    /// Provides a fluent interface for registering services in the dependency injection container.
    /// </summary>
    public interface IServiceRegistry
    {
        /// <summary>
        /// Registers a service with its implementation type and lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        IServiceRegistry Register<TInterface, TImplementation>(ServiceLifetime lifetime) 
            where TImplementation : class, TInterface;

        /// <summary>
        /// Registers a service with its implementation type and lifetime.
        /// </summary>
        /// <param name="serviceType">The service interface type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        IServiceRegistry Register(Type serviceType, Type implementationType, ServiceLifetime lifetime);

        /// <summary>
        /// Registers a service with a specific instance.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="instance">The service instance.</param>
        /// <returns>The service registry for method chaining.</returns>
        IServiceRegistry RegisterInstance<TInterface>(TInterface instance) where TInterface : class;

        /// <summary>
        /// Registers a service with a factory function.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="factory">The factory function to create the service.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        IServiceRegistry RegisterFactory<TInterface>(Func<IServiceProvider, TInterface> factory, ServiceLifetime lifetime) 
            where TInterface : class;

        /// <summary>
        /// Registers a service with a factory function.
        /// </summary>
        /// <param name="serviceType">The service interface type.</param>
        /// <param name="factory">The factory function to create the service.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        IServiceRegistry RegisterFactory(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime);

        /// <summary>
        /// Registers multiple services with the same implementation.
        /// </summary>
        /// <param name="serviceTypes">The service interface types.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        IServiceRegistry RegisterMultiple(IEnumerable<Type> serviceTypes, Type implementationType, ServiceLifetime lifetime);

        /// <summary>
        /// Registers a service as itself (self-registration).
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        IServiceRegistry RegisterSelf<TService>(ServiceLifetime lifetime) where TService : class;

        /// <summary>
        /// Registers a service as itself (self-registration).
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        IServiceRegistry RegisterSelf(Type serviceType, ServiceLifetime lifetime);

        /// <summary>
        /// Replaces an existing service registration.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        IServiceRegistry Rebind<TInterface, TImplementation>(ServiceLifetime lifetime) 
            where TImplementation : class, TInterface;

        /// <summary>
        /// Replaces an existing service registration.
        /// </summary>
        /// <param name="serviceType">The service interface type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        IServiceRegistry Rebind(Type serviceType, Type implementationType, ServiceLifetime lifetime);
    }
}
