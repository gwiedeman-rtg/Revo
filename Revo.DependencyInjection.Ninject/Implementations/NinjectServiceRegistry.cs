using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Ninject.Syntax;
using Revo.DependencyInjection.Core;

namespace Revo.DependencyInjection.Ninject
{
    /// <summary>
    /// Ninject implementation of the service registry.
    /// </summary>
    public class NinjectServiceRegistry : IServiceRegistry
    {
        private readonly IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectServiceRegistry"/> class.
        /// </summary>
        /// <param name="kernel">The Ninject kernel.</param>
        public NinjectServiceRegistry(IKernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }

        /// <summary>
        /// Gets the underlying Ninject kernel.
        /// </summary>
        public IKernel Kernel => _kernel;

        /// <summary>
        /// Registers a service with its implementation type and lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry Register<TInterface, TImplementation>(ServiceLifetime lifetime) 
            where TImplementation : class, TInterface
        {
            var binding = _kernel.Bind<TInterface>().To<TImplementation>();
            ApplyLifetime(binding, lifetime);
            return this;
        }

        /// <summary>
        /// Registers a service with its implementation type and lifetime.
        /// </summary>
        /// <param name="serviceType">The service interface type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry Register(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            var binding = _kernel.Bind(serviceType).To(implementationType);
            ApplyLifetime(binding, lifetime);
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
            _kernel.Bind<TInterface>().ToConstant(instance);
            return this;
        }

        /// <summary>
        /// Registers a service with a factory function.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="factory">The factory function to create the service.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry RegisterFactory<TInterface>(Func<IServiceProvider, TInterface> factory, ServiceLifetime lifetime) 
            where TInterface : class
        {
            var binding = _kernel.Bind<TInterface>().ToMethod(ctx => factory(ctx.Kernel));
            ApplyLifetime(binding, lifetime);
            return this;
        }

        /// <summary>
        /// Registers a service with a factory function.
        /// </summary>
        /// <param name="serviceType">The service interface type.</param>
        /// <param name="factory">The factory function to create the service.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry RegisterFactory(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime)
        {
            var binding = _kernel.Bind(serviceType).ToMethod(ctx => factory(ctx.Kernel));
            ApplyLifetime(binding, lifetime);
            return this;
        }

        /// <summary>
        /// Registers multiple services with the same implementation.
        /// </summary>
        /// <param name="serviceTypes">The service interface types.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry RegisterMultiple(IEnumerable<Type> serviceTypes, Type implementationType, ServiceLifetime lifetime)
        {
            var types = serviceTypes.ToList();
            if (types.Count == 0)
            {
                return this;
            }

            var binding = _kernel.Bind(types.ToArray()).To(implementationType);
            ApplyLifetime(binding, lifetime);
            return this;
        }

        /// <summary>
        /// Registers a service as itself (self-registration).
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry RegisterSelf<TService>(ServiceLifetime lifetime) where TService : class
        {
            var binding = _kernel.Bind<TService>().ToSelf();
            ApplyLifetime(binding, lifetime);
            return this;
        }

        /// <summary>
        /// Registers a service as itself (self-registration).
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry RegisterSelf(Type serviceType, ServiceLifetime lifetime)
        {
            var binding = _kernel.Bind(serviceType).ToSelf();
            ApplyLifetime(binding, lifetime);
            return this;
        }

        /// <summary>
        /// Replaces an existing service registration.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry Rebind<TInterface, TImplementation>(ServiceLifetime lifetime) 
            where TImplementation : class, TInterface
        {
            var binding = _kernel.Rebind<TInterface>().To<TImplementation>();
            ApplyLifetime(binding, lifetime);
            return this;
        }

        /// <summary>
        /// Replaces an existing service registration.
        /// </summary>
        /// <param name="serviceType">The service interface type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public IServiceRegistry Rebind(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            var binding = _kernel.Rebind(serviceType).To(implementationType);
            ApplyLifetime(binding, lifetime);
            return this;
        }

        private void ApplyLifetime(IBindingInSyntax<object> binding, ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    binding.InSingletonScope();
                    break;
                case ServiceLifetime.Scoped:
                    binding.InTaskScope(); // Using TaskScope as equivalent to Scoped
                    break;
                case ServiceLifetime.Transient:
                    binding.InTransientScope();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Unknown service lifetime.");
            }
        }
    }
}
