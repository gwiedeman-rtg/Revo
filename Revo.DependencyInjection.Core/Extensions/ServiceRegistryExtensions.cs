using System;
using System.Collections.Generic;
using System.Linq;

namespace Revo.DependencyInjection.Core
{
    /// <summary>
    /// Extension methods for <see cref="IServiceRegistry"/>.
    /// </summary>
    public static class ServiceRegistryExtensions
    {
        /// <summary>
        /// Registers a service with a factory function using transient lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <param name="factory">The factory function to create the service.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterTransient<TInterface>(this IServiceRegistry registry, Func<IServiceProvider, TInterface> factory) 
            where TInterface : class
        {
            return registry.RegisterFactory(factory, ServiceLifetime.Transient);
        }

        /// <summary>
        /// Registers a service with a factory function using scoped lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <param name="factory">The factory function to create the service.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterScoped<TInterface>(this IServiceRegistry registry, Func<IServiceProvider, TInterface> factory) 
            where TInterface : class
        {
            return registry.RegisterFactory(factory, ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Registers a service with a factory function using singleton lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <param name="factory">The factory function to create the service.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterSingleton<TInterface>(this IServiceRegistry registry, Func<IServiceProvider, TInterface> factory) 
            where TInterface : class
        {
            return registry.RegisterFactory(factory, ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Registers a service with its implementation type using transient lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterTransient<TInterface, TImplementation>(this IServiceRegistry registry) 
            where TImplementation : class, TInterface
        {
            return registry.Register<TInterface, TImplementation>(ServiceLifetime.Transient);
        }

        /// <summary>
        /// Registers a service with its implementation type using scoped lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterScoped<TInterface, TImplementation>(this IServiceRegistry registry) 
            where TImplementation : class, TInterface
        {
            return registry.Register<TInterface, TImplementation>(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Registers a service with its implementation type using singleton lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterSingleton<TInterface, TImplementation>(this IServiceRegistry registry) 
            where TImplementation : class, TInterface
        {
            return registry.Register<TInterface, TImplementation>(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Registers a service as itself using transient lifetime.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterTransientSelf<TService>(this IServiceRegistry registry) 
            where TService : class
        {
            return registry.RegisterSelf<TService>(ServiceLifetime.Transient);
        }

        /// <summary>
        /// Registers a service as itself using scoped lifetime.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterScopedSelf<TService>(this IServiceRegistry registry) 
            where TService : class
        {
            return registry.RegisterSelf<TService>(ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Registers a service as itself using singleton lifetime.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterSingletonSelf<TService>(this IServiceRegistry registry) 
            where TService : class
        {
            return registry.RegisterSelf<TService>(ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Registers multiple services with the same implementation using transient lifetime.
        /// </summary>
        /// <param name="registry">The service registry.</param>
        /// <param name="serviceTypes">The service interface types.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterTransientMultiple(this IServiceRegistry registry, IEnumerable<Type> serviceTypes, Type implementationType)
        {
            return registry.RegisterMultiple(serviceTypes, implementationType, ServiceLifetime.Transient);
        }

        /// <summary>
        /// Registers multiple services with the same implementation using scoped lifetime.
        /// </summary>
        /// <param name="registry">The service registry.</param>
        /// <param name="serviceTypes">The service interface types.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterScopedMultiple(this IServiceRegistry registry, IEnumerable<Type> serviceTypes, Type implementationType)
        {
            return registry.RegisterMultiple(serviceTypes, implementationType, ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Registers multiple services with the same implementation using singleton lifetime.
        /// </summary>
        /// <param name="registry">The service registry.</param>
        /// <param name="serviceTypes">The service interface types.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterSingletonMultiple(this IServiceRegistry registry, IEnumerable<Type> serviceTypes, Type implementationType)
        {
            return registry.RegisterMultiple(serviceTypes, implementationType, ServiceLifetime.Singleton);
        }
    }
}
