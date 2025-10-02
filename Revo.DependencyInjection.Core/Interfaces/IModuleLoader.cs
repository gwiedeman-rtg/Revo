using System;
using System.Collections.Generic;
using System.Reflection;

namespace Revo.DependencyInjection.Core
{
    /// <summary>
    /// Provides functionality to load and manage dependency injection modules.
    /// </summary>
    public interface IModuleLoader
    {
        /// <summary>
        /// Loads all dependency modules from the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for modules.</param>
        void LoadModules(IEnumerable<Assembly> assemblies);

        /// <summary>
        /// Loads a specific module type.
        /// </summary>
        /// <typeparam name="TModule">The module type to load.</typeparam>
        void LoadModule<TModule>() where TModule : class, IDependencyModule, new();

        /// <summary>
        /// Loads a specific module instance.
        /// </summary>
        /// <param name="module">The module instance to load.</param>
        void LoadModule(IDependencyModule module);

        /// <summary>
        /// Loads a specific module type with constructor parameters.
        /// </summary>
        /// <typeparam name="TModule">The module type to load.</typeparam>
        /// <param name="constructorArgs">Constructor arguments for the module.</param>
        void LoadModule<TModule>(params object[] constructorArgs) where TModule : class, IDependencyModule;

        /// <summary>
        /// Gets all loaded modules.
        /// </summary>
        /// <returns>All currently loaded modules.</returns>
        IEnumerable<IDependencyModule> GetLoadedModules();

        /// <summary>
        /// Checks if a module is loaded.
        /// </summary>
        /// <param name="moduleName">The name of the module to check.</param>
        /// <returns>True if the module is loaded, false otherwise.</returns>
        bool IsModuleLoaded(string moduleName);

        /// <summary>
        /// Checks if a module is loaded.
        /// </summary>
        /// <typeparam name="TModule">The module type to check.</typeparam>
        /// <returns>True if the module is loaded, false otherwise.</returns>
        bool IsModuleLoaded<TModule>() where TModule : class, IDependencyModule;
    }
}
