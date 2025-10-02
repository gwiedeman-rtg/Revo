namespace Revo.DependencyInjection.Core
{
    /// <summary>
    /// Specifies the lifetime of a service in the dependency injection container.
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// Specifies that a single instance of the service will be created and shared across all requests.
        /// </summary>
        Singleton,

        /// <summary>
        /// Specifies that a new instance of the service will be created for each scope.
        /// </summary>
        Scoped,

        /// <summary>
        /// Specifies that a new instance of the service will be created for each request.
        /// </summary>
        Transient
    }
}
