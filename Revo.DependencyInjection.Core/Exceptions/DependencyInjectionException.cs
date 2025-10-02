using System;

namespace Revo.DependencyInjection.Core
{
    /// <summary>
    /// Exception thrown when there is an error in dependency injection operations.
    /// </summary>
    public class DependencyInjectionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionException"/> class.
        /// </summary>
        public DependencyInjectionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public DependencyInjectionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DependencyInjectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
