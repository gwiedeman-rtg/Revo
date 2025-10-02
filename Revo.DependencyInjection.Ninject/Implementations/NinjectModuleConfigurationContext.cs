using System;
using Microsoft.Extensions.Configuration;
using Ninject;
using Revo.DependencyInjection.Core;

namespace Revo.DependencyInjection.Ninject
{
    /// <summary>
    /// Ninject implementation of the module configuration context.
    /// </summary>
    public class NinjectModuleConfigurationContext : IModuleConfigurationContext
    {
        private readonly IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectModuleConfigurationContext"/> class.
        /// </summary>
        /// <param name="kernel">The Ninject kernel.</param>
        /// <param name="configuration">The configuration.</param>
        public NinjectModuleConfigurationContext(IKernel kernel, IConfiguration configuration)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Gets the configuration for the application.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the service provider for resolving dependencies during module configuration.
        /// </summary>
        public IServiceProvider ServiceProvider => _kernel;

        /// <summary>
        /// Checks if a module type is enabled for loading.
        /// </summary>
        /// <param name="moduleType">The module type to check.</param>
        /// <returns>True if the module is enabled, false otherwise.</returns>
        public bool IsModuleEnabled(Type moduleType)
        {
            var autoLoadAttr = moduleType.GetCustomAttribute<AutoLoadModuleAttribute>();
            bool autoLoad = autoLoadAttr?.AutoLoad ?? true;

            // Check configuration overrides
            var configKey = $"DependencyInjection:Modules:{moduleType.FullName}:AutoLoad";
            if (Configuration.GetValue<bool?>($"{configKey}") is bool configValue)
            {
                autoLoad = configValue;
            }

            return autoLoad;
        }

        /// <summary>
        /// Checks if a module type is enabled for loading.
        /// </summary>
        /// <typeparam name="TModule">The module type to check.</typeparam>
        /// <returns>True if the module is enabled, false otherwise.</returns>
        public bool IsModuleEnabled<TModule>() where TModule : class, IDependencyModule
        {
            return IsModuleEnabled(typeof(TModule));
        }

        /// <summary>
        /// Gets a configuration section by name.
        /// </summary>
        /// <param name="sectionName">The name of the configuration section.</param>
        /// <returns>The configuration section.</returns>
        public IConfigurationSection GetSection(string sectionName)
        {
            return Configuration.GetSection(sectionName);
        }

        /// <summary>
        /// Gets a configuration section by type.
        /// </summary>
        /// <typeparam name="TSection">The type of the configuration section.</typeparam>
        /// <returns>The configuration section.</returns>
        public TSection GetSection<TSection>() where TSection : class, new()
        {
            var sectionName = typeof(TSection).Name;
            if (sectionName.EndsWith("ConfigurationSection"))
            {
                sectionName = sectionName.Substring(0, sectionName.Length - "ConfigurationSection".Length);
            }
            else if (sectionName.EndsWith("Configuration"))
            {
                sectionName = sectionName.Substring(0, sectionName.Length - "Configuration".Length);
            }

            var section = Configuration.GetSection(sectionName);
            var result = new TSection();
            section.Bind(result);
            return result;
        }
    }
}
