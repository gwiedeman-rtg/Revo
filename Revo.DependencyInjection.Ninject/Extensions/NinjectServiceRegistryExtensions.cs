using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Ninject.Activation;
using Ninject.Syntax;
using Revo.DependencyInjection.Core;

namespace Revo.DependencyInjection.Ninject
{
    /// <summary>
    /// Extension methods for Ninject service registry.
    /// </summary>
    public static class NinjectServiceRegistryExtensions
    {
        /// <summary>
        /// Registers a service with a method that has access to the Ninject context.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <param name="method">The method to create the service.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterMethod<TInterface>(
            this IServiceRegistry registry, 
            Func<IContext, TInterface> method, 
            ServiceLifetime lifetime) 
            where TInterface : class
        {
            if (registry is NinjectServiceRegistry ninjectRegistry)
            {
                var binding = ninjectRegistry.Kernel.Bind<TInterface>().ToMethod(method);
                ApplyLifetime<TInterface>(binding, lifetime);
            }
            else
            {
                // Fallback to factory method
                registry.RegisterFactory(provider => method(null), lifetime);
            }
            
            return registry;
        }

        /// <summary>
        /// Registers a service with a method that has access to the Ninject context.
        /// </summary>
        /// <param name="registry">The service registry.</param>
        /// <param name="serviceType">The service interface type.</param>
        /// <param name="method">The method to create the service.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterMethod(
            this IServiceRegistry registry, 
            Type serviceType, 
            Func<IContext, object> method, 
            ServiceLifetime lifetime)
        {
            if (registry is NinjectServiceRegistry ninjectRegistry)
            {
                var binding = ninjectRegistry.Kernel.Bind(serviceType).ToMethod(method);
                ApplyLifetime<object>(binding, lifetime);
            }
            else
            {
                // Fallback to factory method
                registry.RegisterFactory(serviceType, provider => method(null), lifetime);
            }
            
            return registry;
        }

        /// <summary>
        /// Registers a service with a constant value.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <param name="value">The constant value.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterConstant<TInterface>(
            this IServiceRegistry registry, 
            TInterface value) 
            where TInterface : class
        {
            if (registry is NinjectServiceRegistry ninjectRegistry)
            {
                ninjectRegistry.Kernel.Bind<TInterface>().ToConstant(value);
            }
            else
            {
                registry.RegisterInstance(value);
            }
            
            return registry;
        }

        /// <summary>
        /// Registers a service with a constant value.
        /// </summary>
        /// <param name="registry">The service registry.</param>
        /// <param name="serviceType">The service interface type.</param>
        /// <param name="value">The constant value.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry RegisterConstant(
            this IServiceRegistry registry, 
            Type serviceType, 
            object value)
        {
            if (registry is NinjectServiceRegistry ninjectRegistry)
            {
                ninjectRegistry.Kernel.Bind(serviceType).ToConstant(value);
            }
            else
            {
                registry.RegisterInstance(value);
            }
            
            return registry;
        }

        /// <summary>
        /// Registers a service with a property value.
        /// </summary>
        /// <typeparam name="TInterface">The service interface type.</typeparam>
        /// <param name="registry">The service registry.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The property value.</param>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>The service registry for method chaining.</returns>
        public static IServiceRegistry WithPropertyValue<TInterface>(
            this IServiceRegistry registry, 
            string propertyName, 
            object value, 
            ServiceLifetime lifetime = ServiceLifetime.Transient) 
            where TInterface : class
        {
            if (registry is NinjectServiceRegistry ninjectRegistry)
            {
                var binding = ninjectRegistry.Kernel.Bind<TInterface>().ToSelf();
                binding.WithPropertyValue(propertyName, value);
                ApplyLifetime<TInterface>(binding, lifetime);
            }
            else
            {
                // For non-Ninject registries, we can't set property values directly
                // This would need to be handled by the specific implementation
                registry.RegisterSelf<TInterface>(lifetime);
            }
            
            return registry;
        }

        private static void ApplyLifetime<T>(IBindingInSyntax<T> binding, ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    binding.InSingletonScope();
                    break;
                case ServiceLifetime.Scoped:
                    // For now, use InSingletonScope as a fallback
                    // InTaskScope would require Revo.Core dependency
                    binding.InSingletonScope();
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
