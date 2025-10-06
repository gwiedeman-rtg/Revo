using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Revo.Core.Core
{
    /// <summary>
    /// Simple Microsoft DI adapter for IServiceLocator.
    /// This provides backward compatibility while using Microsoft DI as the primary container.
    /// </summary>
    public class MicrosoftServiceLocator : IServiceLocator
    {
        private readonly IServiceProvider _serviceProvider;

        public MicrosoftServiceLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public object Get(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        public T Get<T>()
        {
            return _serviceProvider.GetService<T>();
        }

        public IEnumerable<object> GetAll(Type serviceType)
        {
            return _serviceProvider.GetServices(serviceType);
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _serviceProvider.GetServices<T>();
        }
    }
}

