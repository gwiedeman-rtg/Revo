# Revo Dependency Injection Ninject

This package provides the Ninject implementation of the Revo dependency injection abstractions. It implements all the core interfaces using Ninject as the underlying DI container.

## Features

- **Full Ninject Integration**: Complete implementation of all DI abstractions
- **Backward Compatibility**: Works with existing Ninject modules
- **Advanced Features**: Supports Ninject-specific features like context preservation
- **Performance Optimized**: Efficient service resolution and scope management
- **Easy Migration**: Simple migration path from existing Ninject code

## Quick Start

### Basic Usage

```csharp
// Create Ninject kernel
var kernel = NinjectDependencyInjectionFactory.CreateKernel();

// Create service container
var container = NinjectDependencyInjectionFactory.CreateServiceContainer(kernel);

// Create module loader
var moduleLoader = NinjectDependencyInjectionFactory.CreateModuleLoader(kernel, configuration, logger);

// Load modules
moduleLoader.LoadModule<MyModule>();

// Resolve services
var service = container.GetService<IMyService>();
```

### Using with Revo Startup

```csharp
// Create Revo configuration
var configuration = new RevoConfiguration();
configuration.ConfigureCoreWithNewDi();

// Create startup
var startup = configuration.CreateRevoStartup(logger);

// Configure with Ninject
var container = startup.ConfigureWithNinject();

// Load modules and run
startup.LoadModulesFromCurrentDomain();
startup.RunApplicationInitialization();
```

## Implementation Details

### NinjectServiceContainer

The `NinjectServiceContainer` wraps a Ninject `IKernel` and provides the `IServiceContainer` interface:

```csharp
public class NinjectServiceContainer : IServiceContainer
{
    private readonly IKernel _kernel;
    
    public T GetService<T>() => _kernel.Get<T>();
    public object GetService(Type serviceType) => _kernel.Get(serviceType);
    // ... other methods
}
```

### NinjectServiceRegistry

The `NinjectServiceRegistry` provides fluent service registration:

```csharp
public class NinjectServiceRegistry : IServiceRegistry
{
    private readonly IKernel _kernel;
    
    public IServiceRegistry Register<TInterface, TImplementation>(ServiceLifetime lifetime)
    {
        var binding = _kernel.Bind<TInterface>().To<TImplementation>();
        ApplyLifetime(binding, lifetime);
        return this;
    }
    // ... other methods
}
```

### NinjectModuleLoader

The `NinjectModuleLoader` handles module discovery and loading:

```csharp
public class NinjectModuleLoader : IModuleLoader
{
    public void LoadModules(IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            LoadModulesFromAssembly(assembly);
        }
    }
    // ... other methods
}
```

## Service Lifetime Mapping

The framework maps service lifetimes to Ninject scopes:

| ServiceLifetime | Ninject Scope | Description |
|----------------|---------------|-------------|
| Singleton | InSingletonScope() | One instance per application |
| Scoped | InTaskScope() | One instance per task/request |
| Transient | InTransientScope() | New instance every time |

## Advanced Features

### Context Preservation

The Ninject implementation supports context preservation for advanced scenarios:

```csharp
// In your module
registry.RegisterMethod<IUnitOfWork>(
    ctx => ctx.ContextPreservingGet<ICommandContext>().UnitOfWork,
    ServiceLifetime.Transient);
```

### Property Injection

You can use property injection with the Ninject extensions:

```csharp
registry.RegisterSingleton<IMyService, MyService>()
    .WithPropertyValue("ConnectionString", "my-connection-string");
```

### Factory Methods

Support for complex factory methods:

```csharp
registry.RegisterMethod<IMyService>(
    ctx => new MyService(ctx.Kernel.Get<IDependency>()),
    ServiceLifetime.Transient);
```

## Migration from Old Ninject System

### Step 1: Update Module Base Class

**Before:**
```csharp
public class MyModule : NinjectModule
{
    public override void Load()
    {
        Bind<IMyService>().To<MyService>().InSingletonScope();
    }
}
```

**After:**
```csharp
public class MyModule : NinjectDependencyModule
{
    public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
    {
        registry.RegisterSingleton<IMyService, MyService>();
    }
}
```

### Step 2: Update Service Registration

**Before:**
```csharp
Bind<IMyService>().To<MyService>().InSingletonScope();
Bind<IMyService>().ToMethod(ctx => new MyService()).InTransientScope();
Bind<IMyService>().ToConstant(instance);
```

**After:**
```csharp
registry.RegisterSingleton<IMyService, MyService>();
registry.RegisterFactory<IMyService>(provider => new MyService(), ServiceLifetime.Transient);
registry.RegisterInstance<IMyService>(instance);
```

### Step 3: Update Service Resolution

**Before:**
```csharp
var service = kernel.Get<IMyService>();
var services = kernel.GetAll<IMyService>();
```

**After:**
```csharp
var service = container.GetService<IMyService>();
var services = container.GetServices<IMyService>();
```

## Configuration

### Module Configuration

You can configure module loading through configuration:

```json
{
  "DependencyInjection": {
    "Modules": {
      "MyNamespace.MyModule": {
        "AutoLoad": true
      }
    }
  }
}
```

### Kernel Configuration

You can configure the Ninject kernel during creation:

```csharp
var kernel = NinjectDependencyInjectionFactory.CreateKernel(kernel =>
{
    // Add Ninject extensions
    kernel.Load(new FuncModule());
    kernel.Load(new ContextPreservationModule());
    
    // Add custom components
    kernel.Components.Add<IBindingResolver, MyCustomResolver>();
});
```

## Testing

### Unit Testing Modules

```csharp
[Test]
public void MyModule_Should_RegisterServices()
{
    // Arrange
    var kernel = new StandardKernel();
    var registry = new NinjectServiceRegistry(kernel);
    var context = new NinjectModuleConfigurationContext(kernel, configuration);
    var module = new MyModule();

    // Act
    module.Configure(registry, context);

    // Assert
    var service = kernel.Get<IMyService>();
    Assert.IsNotNull(service);
    Assert.IsInstanceOf<MyService>(service);
}
```

### Integration Testing

```csharp
[Test]
public void Container_Should_ResolveServices()
{
    // Arrange
    var kernel = NinjectDependencyInjectionFactory.CreateKernel();
    var container = NinjectDependencyInjectionFactory.CreateServiceContainer(kernel);
    var moduleLoader = NinjectDependencyInjectionFactory.CreateModuleLoader(kernel, configuration, logger);
    
    moduleLoader.LoadModule<MyModule>();

    // Act
    var service = container.GetService<IMyService>();

    // Assert
    Assert.IsNotNull(service);
}
```

## Performance Considerations

- **Singleton Services**: Cached for the lifetime of the application
- **Scoped Services**: Cached for the lifetime of the scope
- **Transient Services**: Created fresh each time
- **Module Loading**: Modules are loaded once and cached
- **Service Resolution**: Optimized for common scenarios

## Troubleshooting

### Common Issues

1. **Service Not Found**: Ensure the service is registered in a module
2. **Circular Dependencies**: Use factory methods or property injection
3. **Scope Issues**: Ensure services are registered with appropriate lifetimes
4. **Module Not Loading**: Check the `AutoLoadModule` attribute and configuration

### Debugging

Enable detailed logging to troubleshoot issues:

```csharp
var loggerFactory = LoggerFactory.Create(builder =>
    builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
```

## See Also

- [Revo.DependencyInjection.Core](../Revo.DependencyInjection.Core/README.md) - Core abstractions
- [Ninject Documentation](https://github.com/ninject/Ninject) - Ninject framework
- [Examples](../Examples/README.md) - Usage examples
