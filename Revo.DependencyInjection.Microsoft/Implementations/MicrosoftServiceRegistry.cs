using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Revo.DependencyInjection.Core;
using MSDI = Microsoft.Extensions.DependencyInjection;

namespace Revo.DependencyInjection.Microsoft
{
    /// <summary>
    /// Microsoft.Extensions.DependencyInjection implementation of the service registry.
    /// </summary>
    public class MicrosoftServiceRegistry : IServiceRegistry
    {
        private readonly IServiceCollection _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftServiceRegistry"/> class.
        /// </summary>
        /// <param name="services">The Microsoft service collection.</param>
        public MicrosoftServiceRegistry(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Gets the underlying Microsoft service collection.
        /// </summary>
        public IServiceCollection Services => _services;

        /// <summary>
        /// Registers a service with its implementation type and lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry Register<TInterface, TImplementation>(Revo.DependencyInjection.Core.ServiceLifetime lifetime) 
            where TImplementation : class, TInterface
        {
            var descriptor = new ServiceDescriptor(typeof(TInterface), typeof(TImplementation), MapLifetime(lifetime));
            _services.Add(descriptor);
            return this;
        }

        /// <summary>
        /// Registers a service with its implementation type and lifetime.
        /// </summary>
        /// <param name="serviceType">The service interface type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry Register(Type serviceType, Type implementationType, Revo.DependencyInjection.Core.ServiceLifetime lifetime)
        {
            var descriptor = new ServiceDescriptor(serviceType, implementationType, MapLifetime(lifetime));
            _services.Add(descriptor);
            return this;
        }

        /// <summary>
        /// Registers a service with a specific instance.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="instance">The service instance.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry RegisterInstance<TInterface>(TInterface instance) where TInterface : class
        {
            var descriptor = new ServiceDescriptor(typeof(TInterface), instance);
            _services.Add(descriptor);
            return this;
        }

        /// <summary>
        /// Registers a service with a factory function.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="factory">The factory function.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry RegisterFactory<TInterface>(Func<IServiceProvider, TInterface> factory, Revo.DependencyInjection.Core.ServiceLifetime lifetime) 
            where TInterface : class
        {
            var descriptor = new ServiceDescriptor(typeof(TInterface), provider => factory(provider), MapLifetime(lifetime));
            _services.Add(descriptor);
            return this;
        }

        /// <summary>
        /// Registers a service with a factory function.
        /// </summary>
        /// <param name="serviceType">The service interface type.</param>
        /// <param name="factory">The factory function to create the service.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry RegisterFactory(Type serviceType, Func<IServiceProvider, object> factory, Revo.DependencyInjection.Core.ServiceLifetime lifetime)
        {
            var descriptor = new ServiceDescriptor(serviceType, provider => factory(provider), MapLifetime(lifetime));
            _services.Add(descriptor);
            return this;
        }

        /// <summary>
        /// Registers a service with multiple interfaces.
        /// </summary>
        /// <param name="serviceTypes">The service interface types.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry RegisterMultiple(IEnumerable<Type> serviceTypes, Type implementationType, Revo.DependencyInjection.Core.ServiceLifetime lifetime)
        {
            var mappedLifetime = MapLifetime(lifetime);
            
            foreach (var serviceType in serviceTypes)
            {
                var descriptor = new ServiceDescriptor(serviceType, implementationType, mappedLifetime);
                _services.Add(descriptor);
            }
            
            return this;
        }

        /// <summary>
        /// Registers a service as itself.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry RegisterSelf<TService>(Revo.DependencyInjection.Core.ServiceLifetime lifetime) where TService : class
        {
            var descriptor = new ServiceDescriptor(typeof(TService), typeof(TService), MapLifetime(lifetime));
            _services.Add(descriptor);
            return this;
        }

        /// <summary>
        /// Registers a service as itself (self-registration).
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry RegisterSelf(Type serviceType, Revo.DependencyInjection.Core.ServiceLifetime lifetime)
        {
            var descriptor = new ServiceDescriptor(serviceType, serviceType, MapLifetime(lifetime));
            _services.Add(descriptor);
            return this;
        }

        /// <summary>
        /// Replaces an existing service registration.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry Rebind<TInterface, TImplementation>(Revo.DependencyInjection.Core.ServiceLifetime lifetime) 
            where TImplementation : class, TInterface
        {
            // Remove existing registrations
            var existingDescriptors = _services.Where(s => s.ServiceType == typeof(TInterface)).ToList();
            foreach (var descriptor in existingDescriptors)
            {
                _services.Remove(descriptor);
            }

            // Add new registration
            return Register<TInterface, TImplementation>(lifetime);
        }

        /// <summary>
        /// Replaces an existing service registration.
        /// </summary>
        /// <param name="serviceType">The service interface type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry Rebind(Type serviceType, Type implementationType, Revo.DependencyInjection.Core.ServiceLifetime lifetime)
        {
            // Remove existing registrations
            var existingDescriptors = _services.Where(s => s.ServiceType == serviceType).ToList();
            foreach (var descriptor in existingDescriptors)
            {
                _services.Remove(descriptor);
            }

            // Add new registration
            return Register(serviceType, implementationType, lifetime);
        }

        /// <summary>
        /// Maps Revo service lifetime to Microsoft service lifetime.
        /// </summary>
        /// <param name="lifetime">The Revo service lifetime.</param>
        /// <returns>The Microsoft service lifetime.</returns>
        private static MSDI.ServiceLifetime MapLifetime(Revo.DependencyInjection.Core.ServiceLifetime lifetime)
        {
            return lifetime switch
            {
                Revo.DependencyInjection.Core.ServiceLifetime.Singleton => MSDI.ServiceLifetime.Singleton,
                Revo.DependencyInjection.Core.ServiceLifetime.Scoped => MSDI.ServiceLifetime.Scoped,
                Revo.DependencyInjection.Core.ServiceLifetime.Transient => MSDI.ServiceLifetime.Transient,
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Unknown service lifetime")
            };
        }
    }
}
