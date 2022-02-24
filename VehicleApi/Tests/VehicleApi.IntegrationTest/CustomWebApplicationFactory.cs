namespace VehicleApi.IntegrationTest;

using VehicleApi.Options;
using VehicleApi.Repositories;
using VehicleApi.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    public CustomWebApplicationFactory() => this.ClientOptions.AllowAutoRedirect = false;

    public ApplicationOptions ApplicationOptions { get; private set; } = default!;

    //public Mock<IGreetorService> GreetorServiceMock { get; } = new Mock<IGreetorService>(MockBehavior.Strict);

    public Mock<IGreeterService> GreeterServiceMock { get; } = new Mock<IGreeterService>(MockBehavior.Strict);

    public Mock<ICarRepository> CarRepositoryMock { get; } = new Mock<ICarRepository>(MockBehavior.Strict);

    public Mock<IBikeRepository> BikeRepositoryMock { get; } = new Mock<IBikeRepository>(MockBehavior.Strict);

    public Mock<IClockService> ClockServiceMock { get; } = new Mock<IClockService>(MockBehavior.Strict);

    public void VerifyAllMocks() => Mock.VerifyAll(this.CarRepositoryMock,
        this.ClockServiceMock
        , this.GreeterServiceMock
        );

    protected override void ConfigureClient(HttpClient client)
    {
        using (var serviceScope = this.Services.CreateScope())
        {
            var serviceProvider = serviceScope.ServiceProvider;
            this.ApplicationOptions = serviceProvider.GetRequiredService<ApplicationOptions>();
        }

        base.ConfigureClient(client);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder
            .UseEnvironment(Constants.EnvironmentName.Test)
            .ConfigureServices(this.ConfigureServices);

    protected virtual void ConfigureServices(IServiceCollection services) =>
        services
            .AddSingleton(this.CarRepositoryMock.Object)
            .AddSingleton(this.BikeRepositoryMock.Object)
            .AddSingleton(this.ClockServiceMock.Object)
            .AddSingleton(this.GreeterServiceMock.Object)
        ;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.VerifyAllMocks();
        }

        base.Dispose(disposing);
    }
}
