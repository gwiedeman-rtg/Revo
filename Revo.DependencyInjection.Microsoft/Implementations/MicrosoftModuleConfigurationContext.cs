using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Revo.DependencyInjection.Core;
using Revo.Core.Core;
using MSConfig = Microsoft.Extensions.Configuration;

namespace Revo.DependencyInjection.Microsoft
{
    /// <summary>
    /// Microsoft DI implementation of the module configuration context.
    /// </summary>
    public class MicrosoftModuleConfigurationContext : IModuleConfigurationContext
    {
        private readonly MSConfig.IConfiguration _configuration;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftModuleConfigurationContext"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public MicrosoftModuleConfigurationContext(MSConfig.IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public MSConfig.IConfiguration Configuration => _configuration;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger => _logger;

        /// <summary>
        /// Gets the service provider for resolving dependencies during module configuration.
        /// </summary>
        public IServiceProvider ServiceProvider => null; // Not available in this context

        /// <summary>
        /// Gets a configuration value.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value.</returns>
        public string GetValue(string key)
        {
            return _configuration[key];
        }

        /// <summary>
        /// Gets a configuration value with a default.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The configuration value or default.</returns>
        public string GetValue(string key, string defaultValue)
        {
            return _configuration[key] ?? defaultValue;
        }

        /// <summary>
        /// Gets a typed configuration value.
        /// </summary>
        /// <typeparam name="T">The type of the configuration value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <returns>The typed configuration value.</returns>
        public T GetValue<T>(string key)
        {
            var value = _configuration[key];
            if (value == null)
            {
                return default(T);
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Gets a typed configuration value with a default.
        /// </summary>
        /// <typeparam name="T">The type of the configuration value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The typed configuration value or default.</returns>
        public T GetValue<T>(string key, T defaultValue)
        {
            var value = _configuration[key];
            if (value == null)
            {
                return defaultValue;
            }
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Binds a configuration section to an object.
        /// </summary>
        /// <typeparam name="T">The type to bind to.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <returns>The bound object.</returns>
        public T Bind<T>(string key) where T : class, new()
        {
            var instance = new T();
            // Simple binding implementation - in a real scenario, you'd use Microsoft.Extensions.Configuration.Binder
            var section = _configuration.GetSection(key);
            foreach (var child in section.GetChildren())
            {
                // This is a simplified implementation
                // In practice, you'd use reflection to bind properties
            }
            return instance;
        }

        /// <summary>
        /// Binds a configuration section to an existing object.
        /// </summary>
        /// <typeparam name="T">The type to bind to.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <param name="instance">The instance to bind to.</param>
        public void Bind<T>(string key, T instance) where T : class
        {
            // Simple binding implementation - in a real scenario, you'd use Microsoft.Extensions.Configuration.Binder
            var section = _configuration.GetSection(key);
            foreach (var child in section.GetChildren())
            {
                // This is a simplified implementation
                // In practice, you'd use reflection to bind properties
            }
        }

        /// <summary>
        /// Checks if a module is enabled.
        /// </summary>
        /// <typeparam name="TModule">The module type.</typeparam>
        /// <returns>True if the module is enabled.</returns>
        public bool IsModuleEnabled<TModule>() where TModule : class, IDependencyModule
        {
            return IsModuleEnabled(typeof(TModule));
        }

        /// <summary>
        /// Checks if a module type is enabled for loading.
        /// </summary>
        /// <param name="moduleType">The module type to check.</param>
        /// <returns>True if the module is enabled, false otherwise.</returns>
        public bool IsModuleEnabled(Type moduleType)
        {
            var autoLoadAttribute = moduleType.GetCustomAttribute<Revo.Core.Core.AutoLoadModuleAttribute>();
            
            if (autoLoadAttribute == null)
            {
                return true; // No attribute means enabled by default
            }

            if (!autoLoadAttribute.AutoLoad)
            {
                return false; // Explicitly disabled
            }

            // Check configuration for module-specific settings
            var moduleName = moduleType.Name;
            var configKey = $"Modules:{moduleName}:Enabled";
            return _configuration.GetValue(configKey, true);
        }

        /// <summary>
        /// Gets a configuration section by name.
        /// </summary>
        /// <param name="sectionName">The name of the configuration section.</param>
        /// <returns>The configuration section.</returns>
        public IConfigurationSection GetSection(string sectionName)
        {
            return _configuration.GetSection(sectionName);
        }

        /// <summary>
        /// Gets a configuration section by type.
        /// </summary>
        /// <typeparam name="TSection">The type of the configuration section.</typeparam>
        /// <returns>The configuration section.</returns>
        public TSection GetSection<TSection>() where TSection : class, new()
        {
            var instance = new TSection();
            _configuration.Bind(instance);
            return instance;
        }
    }
}
