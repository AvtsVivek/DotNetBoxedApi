namespace VehicleApi.IntegrationTest.Controllers;

using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using VehicleApi.Services;
using Boxed.AspNetCore;
using Moq;
using Xunit;

public class SimpleTests
{
    [Fact]
    public void SomeTest()
    {
        var greetorServiceMock = new Mock<IGreeterService>(MockBehavior.Strict);

        greetorServiceMock.Setup(x => x.Greeting("Hi")).Returns("Hello");

        var greetorService = greetorServiceMock.Object;

        var helloString = greetorService.Greeting("Hi");

        Assert.Equal("Hello", helloString);
    }
}

public class CustomWebApplicationFactoryTrials : CustomWebApplicationFactory<Program>
{
    private readonly HttpClient client;
    private readonly MediaTypeFormatterCollection formatters;

    public CustomWebApplicationFactoryTrials()
    {
        this.client = this.CreateClient();
        this.formatters = new MediaTypeFormatterCollection();
        this.formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(ContentType.RestfulJson));
        this.formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(ContentType.ProblemJson));
    }

    [Fact]
    public async Task SomeTestAsync()
    {
        this.GreeterServiceMock.Setup(x => x.Greeting("vivek")).Returns("Hello, Good Evening!! How are you");
        var response = await this.client.GetAsync(new Uri("/bikes/Fetch/vivek", UriKind.Relative)).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        Assert.NotNull(jsonString);
    }


    [Fact]
    public void SomeTest()
    {
        var greetorServiceMock = new Mock<IGreeterService>(MockBehavior.Strict);

        greetorServiceMock.Setup(x => x.Greeting("Hi")).Returns("Hello");

        var greetorService = greetorServiceMock.Object;

        var helloString = greetorService.Greeting("Hi");

        Assert.Equal("Hello", helloString);
    }
}
