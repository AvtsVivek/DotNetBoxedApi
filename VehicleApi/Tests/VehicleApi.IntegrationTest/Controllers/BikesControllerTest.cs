namespace VehicleApi.IntegrationTest.Controllers;

using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using VehicleApi.Commands;
using Boxed.AspNetCore;
using Moq;
using Xunit;

public class BikesControllerTest : CustomWebApplicationFactory<Program>
{
    private readonly HttpClient client;
    private readonly MediaTypeFormatterCollection formatters;

    public BikesControllerTest()
    {
        this.client = this.CreateClient();
        this.formatters = new MediaTypeFormatterCollection();
        this.formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(ContentType.RestfulJson));
        this.formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(ContentType.ProblemJson));
    }

    [Fact]
    public async Task Get_BikeFound_Returns200OkAsync()
    {
        var greetingsMessage = "Hello";
        var bikeId = 108;
        var someModel = "Activa";
        var bike = new Models.Bike() { Model = someModel, BikeId = bikeId, Modified = new DateTimeOffset(2000, 1, 2, 3, 4, 5, TimeSpan.FromHours(6)) };
        this.BikeRepositoryMock.Setup(x => x.GetAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(bike);
        this.GreeterServiceMock.Setup(x => x.Greeting(someModel)).Returns(greetingsMessage);
        var response = await this.client.GetAsync(new Uri("/bikes/1", UriKind.Relative)).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new DateTimeOffset(2000, 1, 2, 3, 4, 5, TimeSpan.FromHours(6)), response.Content.Headers.LastModified);
        Assert.Equal(ContentType.RestfulJson, response.Content.Headers.ContentType?.MediaType);
        var bikeWithGreetings = await response.Content.ReadAsAsync<BikeWithGreeting>(this.formatters).ConfigureAwait(false);
        Assert.Equal(bikeId, bikeWithGreetings.Bike.BikeId);
        Assert.Equal(greetingsMessage, bikeWithGreetings.Message);
    }
}
