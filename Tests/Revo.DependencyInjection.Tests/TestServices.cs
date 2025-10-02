using System;

namespace Revo.DependencyInjection.Tests
{
    /// <summary>
    /// Test service interface.
    /// </summary>
    public interface ITestService
    {
        string GetValue();
    }

    /// <summary>
    /// Test service implementation.
    /// </summary>
    public class TestService : ITestService
    {
        public string GetValue() => "Test Value";
    }

    /// <summary>
    /// Another test service interface.
    /// </summary>
    public interface IAnotherTestService
    {
        string GetAnotherValue();
    }

    /// <summary>
    /// Another test service implementation.
    /// </summary>
    public class AnotherTestService : IAnotherTestService
    {
        public string GetAnotherValue() => "Another Test Value";
    }

    /// <summary>
    /// Test service that implements multiple interfaces.
    /// </summary>
    public class MultiInterfaceService : ITestService, IAnotherTestService
    {
        public string GetValue() => "Multi Value";
        public string GetAnotherValue() => "Multi Another Value";
    }

    /// <summary>
    /// Test service with dependencies.
    /// </summary>
    public interface IDependentService
    {
        string GetCombinedValue();
    }

    /// <summary>
    /// Test service with dependencies implementation.
    /// </summary>
    public class DependentService : IDependentService
    {
        private readonly ITestService _testService;
        private readonly IAnotherTestService _anotherTestService;

        public DependentService(ITestService testService, IAnotherTestService anotherTestService)
        {
            _testService = testService ?? throw new ArgumentNullException(nameof(testService));
            _anotherTestService = anotherTestService ?? throw new ArgumentNullException(nameof(anotherTestService));
        }

        public string GetCombinedValue()
        {
            return $"{_testService.GetValue()} - {_anotherTestService.GetAnotherValue()}";
        }
    }

    /// <summary>
    /// Test service with factory method.
    /// </summary>
    public interface IFactoryService
    {
        string GetFactoryValue();
    }

    /// <summary>
    /// Test service with factory method implementation.
    /// </summary>
    public class FactoryService : IFactoryService
    {
        private readonly string _value;

        public FactoryService(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string GetFactoryValue() => _value;
    }
}
