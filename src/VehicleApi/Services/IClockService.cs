namespace VehicleApi.Services;

/// <summary>
/// Retrieves the current date and/or time. Helps with unit testing by letting you mock the system clock.
/// </summary>
public interface IClockService
{
    DateTimeOffset UtcNow { get; }
}

public interface IGreeterService
{
    string Greeting(string name);
}

public class GreeterService : IGreeterService
{
    public string Greeting(string name) => $"Hello {name}, Good Morning!!";
}
