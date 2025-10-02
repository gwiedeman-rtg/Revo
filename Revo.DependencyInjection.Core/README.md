# Revo Dependency Injection Core

This package provides the core abstractions for dependency injection in the Revo framework. It defines interfaces and models that are independent of any specific DI container implementation, allowing you to use any DI container with Revo.

## Features

- **Container Agnostic**: Works with any DI container implementation
- **Fluent API**: Easy-to-use registration methods
- **Module System**: Organized service registration through modules
- **Scope Management**: Proper lifetime management for services
- **Configuration Integration**: Seamless integration with configuration systems
- **Testing Support**: Easy to mock and test

## Core Interfaces

### IServiceContainer
The primary interface for resolving services from the container.

```csharp
public interface IServiceContainer
{
    T GetService<T>();
    object GetService(Type serviceType);
    IEnumerable<T> GetServices<T>();
    IEnumerable<object> GetServices(Type serviceType);
    IServiceScope CreateScope();
    T GetServiceOrDefault<T>();
    object GetServiceOrDefault(Type serviceType);
}
```

### IServiceRegistry
Provides a fluent interface for registering services.

```csharp
public interface IServiceRegistry
{
    IServiceRegistry Register<TInterface, TImplementation>(ServiceLifetime lifetime);
    IServiceRegistry Register(Type serviceType, Type implementationType, ServiceLifetime lifetime);
    IServiceRegistry RegisterInstance<TInterface>(TInterface instance);
    IServiceRegistry RegisterFactory<TInterface>(Func<IServiceProvider, TInterface> factory, ServiceLifetime lifetime);
    IServiceRegistry RegisterMultiple(IEnumerable<Type> serviceTypes, Type implementationType, ServiceLifetime lifetime);
    IServiceRegistry RegisterSelf<TService>(ServiceLifetime lifetime);
    IServiceRegistry Rebind<TInterface, TImplementation>(ServiceLifetime lifetime);
}
```

### IDependencyModule
Base interface for dependency injection modules.

```csharp
public interface IDependencyModule
{
    string Name { get; }
    bool AutoLoad { get; }
    void Configure(IServiceRegistry registry, IModuleConfigurationContext context);
}
```

### IModuleLoader
Manages loading and discovery of dependency modules.

```csharp
public interface IModuleLoader
{
    void LoadModules(IEnumerable<Assembly> assemblies);
    void LoadModule<TModule>() where TModule : class, IDependencyModule, new();
    void LoadModule(IDependencyModule module);
    void LoadModule<TModule>(params object[] constructorArgs) where TModule : class, IDependencyModule;
    IEnumerable<IDependencyModule> GetLoadedModules();
    bool IsModuleLoaded(string moduleName);
    bool IsModuleLoaded<TModule>() where TModule : class, IDependencyModule;
}
```

## Service Lifetimes

The framework supports three service lifetimes:

- **Singleton**: One instance per application
- **Scoped**: One instance per scope (e.g., per request)
- **Transient**: New instance every time

## Usage Examples

### Creating a Module

```csharp
public class MyModule : DependencyModuleBase
{
    public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
    {
        // Register services
        registry.RegisterSingleton<IMyService, MyService>();
        registry.RegisterScoped<IMyRepository, MyRepository>();
        registry.RegisterFactory<IMyFactory>(provider => new MyFactory(), ServiceLifetime.Transient);
        
        // Register multiple interfaces
        registry.RegisterMultiple(
            new[] { typeof(IMyService), typeof(IMyOtherService) },
            typeof(MyService),
            ServiceLifetime.Singleton);
    }
}
```

### Using Extension Methods

```csharp
// Fluent registration methods
registry.RegisterSingleton<IMyService, MyService>();
registry.RegisterScoped<IMyRepository, MyRepository>();
registry.RegisterTransient<IMyFactory, MyFactory>();

// Self-registration
registry.RegisterSelf<MyService>(ServiceLifetime.Singleton);

// Factory registration
registry.RegisterFactory<IMyService>(provider => new MyService(), ServiceLifetime.Transient);
```

### Module Discovery

Modules are automatically discovered using the `AutoLoadModuleAttribute`:

```csharp
[AutoLoadModule(true)]  // Auto-load by default
public class MyModule : DependencyModuleBase
{
    // Module implementation
}

[AutoLoadModule(false)] // Disabled by default
public class OptionalModule : DependencyModuleBase
{
    // Module implementation
}
```

## Configuration Integration

Modules can access configuration through the `IModuleConfigurationContext`:

```csharp
public override void Configure(IServiceRegistry registry, IModuleConfigurationContext context)
{
    // Get configuration section
    var myConfig = context.GetSection<MyConfigurationSection>();
    
    // Check if module is enabled
    if (context.IsModuleEnabled<MyModule>())
    {
        // Register services
    }
}
```

## Error Handling

The framework provides specific exception types for DI-related errors:

- `DependencyInjectionException`: General DI errors
- `ServiceNotRegisteredException`: Service not found errors

## Testing

The abstraction makes it easy to test your modules:

```csharp
[Test]
public void MyModule_Should_RegisterServices()
{
    // Arrange
    var mockRegistry = new Mock<IServiceRegistry>();
    var mockContext = new Mock<IModuleConfigurationContext>();
    var module = new MyModule();

    // Act
    module.Configure(mockRegistry.Object, mockContext.Object);

    // Assert
    mockRegistry.Verify(r => r.RegisterSingleton<IMyService, MyService>(), Times.Once);
}
```

## Best Practices

1. **Use Modules**: Organize service registrations into modules
2. **Choose Appropriate Lifetimes**: Use the correct lifetime for each service
3. **Use Factories for Complex Creation**: Use factory methods for complex object creation
4. **Test Your Modules**: Write unit tests for your modules
5. **Use Configuration**: Leverage configuration for module behavior
6. **Handle Errors Gracefully**: Use the provided exception types

## Migration from Ninject

If you're migrating from the old Ninject-based system:

1. Change your module base class from `NinjectModule` to `NinjectDependencyModule`
2. Update your `Load()` method to `Configure(IServiceRegistry, IModuleConfigurationContext)`
3. Replace Ninject-specific binding syntax with the fluent API
4. Use the new service lifetime enum instead of Ninject scopes

## See Also

- [Revo.DependencyInjection.Ninject](../Revo.DependencyInjection.Ninject/README.md) - Ninject implementation
- [Revo Configuration](../Revo.Core/Configuration/README.md) - Configuration system
- [Examples](../Examples/README.md) - Usage examples
