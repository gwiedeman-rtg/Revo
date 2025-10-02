using System;

namespace Revo.DependencyInjection.Core
{
    /// <summary>
    /// Exception thrown when a requested service is not registered in the container.
    /// </summary>
    public class ServiceNotRegisteredException : DependencyInjectionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotRegisteredException"/> class.
        /// </summary>
        /// <param name="serviceType">The type of service that was not registered.</param>
        public ServiceNotRegisteredException(Type serviceType) 
            : base($"Service of type '{serviceType.FullName}' is not registered.")
        {
            ServiceType = serviceType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotRegisteredException"/> class.
        /// </summary>
        /// <param name="serviceType">The type of service that was not registered.</param>
        /// <param name="message">The error message.</param>
        public ServiceNotRegisteredException(Type serviceType, string message) 
            : base(message)
        {
            ServiceType = serviceType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotRegisteredException"/> class.
        /// </summary>
        /// <param name="serviceType">The type of service that was not registered.</param>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServiceNotRegisteredException(Type serviceType, string message, Exception innerException) 
            : base(message, innerException)
        {
            ServiceType = serviceType;
        }

        /// <summary>
        /// Gets the type of service that was not registered.
        /// </summary>
        public Type ServiceType { get; }
    }
}
