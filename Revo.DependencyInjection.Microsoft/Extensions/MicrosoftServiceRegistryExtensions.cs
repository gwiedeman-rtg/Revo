using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Revo.DependencyInjection.Core;
using MSDI = Microsoft.Extensions.DependencyInjection;

namespace Revo.DependencyInjection.Microsoft
{
    /// <summary>
    /// Extension methods for Microsoft DI service registry.
    /// </summary>
    public static class MicrosoftServiceRegistryExtensions
    {
        /// <summary>
        /// Registers a service as singleton.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterSingleton<TInterface, TImplementation>(this IServiceRegistry registry) 
            where TImplementation : class, TInterface
        {
            return registry.Register<TInterface, TImplementation>(Revo.DependencyInjection.Core.ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Registers a service as scoped.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterScoped<TInterface, TImplementation>(this IServiceRegistry registry) 
            where TImplementation : class, TInterface
        {
            return registry.Register<TInterface, TImplementation>(Revo.DependencyInjection.Core.ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Registers a service as transient.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterTransient<TInterface, TImplementation>(this IServiceRegistry registry) 
            where TImplementation : class, TInterface
        {
            return registry.Register<TInterface, TImplementation>(Revo.DependencyInjection.Core.ServiceLifetime.Transient);
        }

        /// <summary>
        /// Registers a service as singleton.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterSingleton<TService>(this IServiceRegistry registry) 
            where TService : class
        {
            return registry.RegisterSelf<TService>(Revo.DependencyInjection.Core.ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Registers a service as scoped.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterScoped<TService>(this IServiceRegistry registry) 
            where TService : class
        {
            return registry.RegisterSelf<TService>(Revo.DependencyInjection.Core.ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Registers a service as transient.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterTransient<TService>(this IServiceRegistry registry) 
            where TService : class
        {
            return registry.RegisterSelf<TService>(Revo.DependencyInjection.Core.ServiceLifetime.Transient);
        }

        /// <summary>
        /// Registers a factory service as singleton.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <param name="factory">The factory function.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterSingleton<TInterface>(this IServiceRegistry registry, Func<IServiceProvider, TInterface> factory) 
            where TInterface : class
        {
            return registry.RegisterFactory(factory, Revo.DependencyInjection.Core.ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Registers a factory service as scoped.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <param name="factory">The factory function.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterScoped<TInterface>(this IServiceRegistry registry, Func<IServiceProvider, TInterface> factory) 
            where TInterface : class
        {
            return registry.RegisterFactory(factory, Revo.DependencyInjection.Core.ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Registers a factory service as transient.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <param name="factory">The factory function.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterTransient<TInterface>(this IServiceRegistry registry, Func<IServiceProvider, TInterface> factory) 
            where TInterface : class
        {
            return registry.RegisterFactory(factory, Revo.DependencyInjection.Core.ServiceLifetime.Transient);
        }

        /// <summary>
        /// Registers multiple services with the same implementation as singleton.
        /// </summary>
        /// <param name="registry">The service registry.</param>
        /// <param name="serviceTypes">The service interface types.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterSingleton(this IServiceRegistry registry, IEnumerable<Type> serviceTypes, Type implementationType)
        {
            return registry.RegisterMultiple(serviceTypes, implementationType, Revo.DependencyInjection.Core.ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Registers multiple services with the same implementation as scoped.
        /// </summary>
        /// <param name="registry">The service registry.</param>
        /// <param name="serviceTypes">The service interface types.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterScoped(this IServiceRegistry registry, IEnumerable<Type> serviceTypes, Type implementationType)
        {
            return registry.RegisterMultiple(serviceTypes, implementationType, Revo.DependencyInjection.Core.ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Registers multiple services with the same implementation as transient.
        /// </summary>
        /// <param name="registry">The service registry.</param>
        /// <param name="serviceTypes">The service interface types.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterTransient(this IServiceRegistry registry, IEnumerable<Type> serviceTypes, Type implementationType)
        {
            return registry.RegisterMultiple(serviceTypes, implementationType, Revo.DependencyInjection.Core.ServiceLifetime.Transient);
        }
    }
}
