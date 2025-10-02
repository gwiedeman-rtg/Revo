using System;

namespace Revo.DependencyInjection.Core
{
    /// <summary>
    /// Specifies whether a dependency module should be automatically loaded.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class AutoLoadModuleAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoLoadModuleAttribute"/> class.
        /// </summary>
        /// <param name="autoLoad">True if the module should be automatically loaded, false otherwise.</param>
        public AutoLoadModuleAttribute(bool autoLoad = true)
        {
            AutoLoad = autoLoad;
        }

        /// <summary>
        /// Gets a value indicating whether the module should be automatically loaded.
        /// </summary>
        public bool AutoLoad { get; }
    }
}
