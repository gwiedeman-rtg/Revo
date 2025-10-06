using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Revo.Core.Core;
using Revo.Core.Lifecycle;
using MSConfig = Microsoft.Extensions.Configuration;

namespace Revo.Core.Configuration
{
    /// <summary>
    /// Simplified Revo startup using Microsoft DI as the primary container.
    /// This eliminates the complex abstractions and uses standard .NET patterns.
    /// </summary>
    public class RevoMicrosoftDIStartup
    {
        private readonly IServiceCollection _services;
        private readonly MSConfig.IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly HashSet<Assembly> _loadedAssemblies = new HashSet<Assembly>();

        public RevoMicrosoftDIStartup(
            IServiceCollection services, 
            MSConfig.IConfiguration configuration, 
            ILogger logger)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Configures Revo services using Microsoft DI.
        /// </summary>
        public void Configure()
        {
            // Register core Revo services
            RegisterCoreServices();
            
            // Load and configure modules from assemblies
            LoadModules();
            
            // Register service locator for backward compatibility
            _services.AddSingleton<IServiceLocator>(provider => 
                new MicrosoftServiceLocator(provider));
        }

        /// <summary>
        /// Registers core Revo services.
        /// </summary>
        private void RegisterCoreServices()
        {
            // Register core services directly with Microsoft DI
            _services.AddSingleton<MSConfig.IConfiguration>(_configuration);
            _services.AddSingleton<ILogger>(_logger);
            
            // Register any other core services here
            // This replaces the complex module system with simple service registration
        }

        /// <summary>
        /// Loads modules from assemblies using reflection.
        /// This is much simpler than the complex module loading system.
        /// </summary>
        private void LoadModules()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .ToList();

            foreach (var assembly in assemblies)
            {
                LoadModulesFromAssembly(assembly);
            }
        }

        /// <summary>
        /// Loads modules from a specific assembly.
        /// </summary>
        private void LoadModulesFromAssembly(Assembly assembly)
        {
            if (_loadedAssemblies.Contains(assembly))
                return;

            try
            {
                // Find classes that implement IRevoModule (simplified interface)
                var moduleTypes = assembly.GetTypes()
                    .Where(t => typeof(IRevoModule).IsAssignableFrom(t) && 
                               !t.IsAbstract && 
                               !t.IsInterface)
                    .ToList();

                foreach (var moduleType in moduleTypes)
                {
                    var module = Activator.CreateInstance(moduleType) as IRevoModule;
                    if (module != null)
                    {
                        _logger.LogInformation($"Loading module: {moduleType.Name}");
                        module.Configure(_services, _configuration, _logger);
                    }
                }

                _loadedAssemblies.Add(assembly);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to load modules from assembly: {assembly.FullName}");
            }
        }
    }

    /// <summary>
    /// Simplified module interface that uses Microsoft DI directly.
    /// This replaces the complex IDependencyModule system.
    /// </summary>
    public interface IRevoModule
    {
        void Configure(IServiceCollection services, MSConfig.IConfiguration configuration, ILogger logger);
    }
}
