namespace VehicleApi.IntegrationTest.Controllers;

using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Boxed.AspNetCore;
using Moq;
using Xunit;

public class CarsControllerGetPageTests : CustomWebApplicationFactory<Program>
{
    private readonly HttpClient client;
    private readonly MediaTypeFormatterCollection formatters;

    public CarsControllerGetPageTests()
    {
        this.client = this.CreateClient();
        this.formatters = new MediaTypeFormatterCollection();
        this.formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(ContentType.RestfulJson));
        this.formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(ContentType.ProblemJson));
    }

    protected override void ConfigureServices(IServiceCollection services) =>
    services
        .AddSingleton(this.CarRepositoryMock.Object)
        .AddSingleton(this.GreeterServiceMock.Object);

    [Fact]
    public async Task GetPage_SecondPage_Returns200OkAsync()
    {
        this.GreeterServiceMock.Setup(x => x.Greeting(It.IsAny<string>())).Returns(It.IsAny<string>());
        var cars = TestUtilities.GetCars();
        this.CarRepositoryMock
            .Setup(x => x.GetCarsAsync(3, new DateTimeOffset(2000, 1, 3, 0, 0, 0, TimeSpan.Zero), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cars.Skip(3).Take(3).ToList());
        this.CarRepositoryMock
            .Setup(x => x.GetTotalCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cars.Count);
        this.CarRepositoryMock
            .Setup(x => x.GetHasNextPageAsync(3, new DateTimeOffset(2000, 1, 3, 0, 0, 0, TimeSpan.Zero), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var response = await this.client
            .GetAsync(new Uri($"/cars?First=3&After={Cursor.ToCursor(new DateTimeOffset(2000, 1, 3, 0, 0, 0, TimeSpan.Zero))}", UriKind.Relative))
            .ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(ContentType.RestfulJson, response.Content.Headers.ContentType?.MediaType);
        await TestUtilities.AssertPageUrlsAsync(
                response,
                nextPageUrl: null,
                previousPageUrl: $"http://localhost/cars?Last=3&Before={Cursor.ToCursor(new DateTimeOffset(2000, 1, 4, 0, 0, 0, TimeSpan.Zero))}",
                expectedPageCount: 3,
                actualPageCount: 1,
                totalCount: 4,
                this.formatters
                )
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task GetPage_SecondLastPage_Returns200OkAsync()
    {
        this.GreeterServiceMock.Setup(x => x.Greeting(It.IsAny<string>())).Returns(It.IsAny<string>());
        var cars = TestUtilities.GetCars();
        this.CarRepositoryMock
            .Setup(x => x.GetCarsReverseAsync(3, null, new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.Zero), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cars.SkipLast(3).TakeLast(3).ToList());
        this.CarRepositoryMock
            .Setup(x => x.GetTotalCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cars.Count);
        this.CarRepositoryMock
            .Setup(x => x.GetHasPreviousPageAsync(3, new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.Zero), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var response = await this.client
            .GetAsync(new Uri($"/cars?Last=3&Before={Cursor.ToCursor(new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.Zero))}", UriKind.Relative))
            .ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(ContentType.RestfulJson, response.Content.Headers.ContentType?.MediaType);
        await TestUtilities.AssertPageUrlsAsync(
                response,
                nextPageUrl: $"http://localhost/cars?First=3&After={Cursor.ToCursor(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero))}",
                previousPageUrl: null,
                expectedPageCount: 3,
                actualPageCount: 1,
                totalCount: 4,
                this.formatters
                )
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task GetPage_MiddlePage_Returns200OkAsync()
    {
        this.GreeterServiceMock.Setup(x => x.Greeting(It.IsAny<string>())).Returns(It.IsAny<string>());
        var cars = TestUtilities.GetCars();
        this.CarRepositoryMock
            .Setup(x => x.GetCarsAsync(2, new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cars.Skip(1).Take(2).ToList());
        this.CarRepositoryMock
            .Setup(x => x.GetTotalCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cars.Count);
        this.CarRepositoryMock
            .Setup(x => x.GetHasNextPageAsync(2, new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var response = await this.client
            .GetAsync(new Uri($"/cars?First=2&After={Cursor.ToCursor(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero))}", UriKind.Relative))
            .ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(ContentType.RestfulJson, response.Content.Headers.ContentType?.MediaType);
        await TestUtilities.AssertPageUrlsAsync(
                response,
                nextPageUrl: $"http://localhost/cars?First=2&After={Cursor.ToCursor(new DateTimeOffset(2000, 1, 3, 0, 0, 0, TimeSpan.Zero))}",
                previousPageUrl: $"http://localhost/cars?Last=2&Before={Cursor.ToCursor(new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.Zero))}",
                expectedPageCount: 2,
                actualPageCount: 2,
                totalCount: 4,
                this.formatters
                )
            .ConfigureAwait(false);
    }

    [Theory]
    [InlineData("/cars?last=3")]
    [InlineData("/cars?last=3&before=THIS_IS_INVALID")]
    public async Task GetPage_LastPage_Returns200OkAsync(string path)
    {
        this.GreeterServiceMock.Setup(x => x.Greeting(It.IsAny<string>())).Returns(It.IsAny<string>());
        var cars = TestUtilities.GetCars();
        this.CarRepositoryMock
            .Setup(x => x.GetCarsReverseAsync(3, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cars.TakeLast(3).ToList());
        this.CarRepositoryMock
            .Setup(x => x.GetTotalCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cars.Count);
        this.CarRepositoryMock
            .Setup(x => x.GetHasPreviousPageAsync(3, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var response = await this.client.GetAsync(new Uri(path, UriKind.Relative)).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(ContentType.RestfulJson, response.Content.Headers.ContentType?.MediaType);
        await TestUtilities.AssertPageUrlsAsync(
                response,
                nextPageUrl: null,
                previousPageUrl: $"http://localhost/cars?Last=3&Before={Cursor.ToCursor(new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.Zero))}",
                expectedPageCount: 3,
                actualPageCount: 3,
                totalCount: 4,
                this.formatters
                )
            .ConfigureAwait(false);
    }

    [Theory]
    [InlineData("/cars")]
    [InlineData("/cars?first=3")]
    [InlineData("/cars?first=3&after=THIS_IS_INVALID")]
    public async Task GetPage_FirstPage_Returns200OkAsync(string path)
    {
        this.GreeterServiceMock.Setup(x => x.Greeting(It.IsAny<string>())).Returns(It.IsAny<string>());
        var cars = TestUtilities.GetCars();
        this.CarRepositoryMock
            .Setup(x => x.GetCarsAsync(3, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cars.Take(3).ToList());
        this.CarRepositoryMock
            .Setup(x => x.GetTotalCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cars.Count);
        this.CarRepositoryMock
            .Setup(x => x.GetHasNextPageAsync(3, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var response = await this.client.GetAsync(new Uri(path, UriKind.Relative)).ConfigureAwait(false);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(ContentType.RestfulJson, response.Content.Headers.ContentType?.MediaType);
        await TestUtilities.AssertPageUrlsAsync(
                response,
                nextPageUrl: $"http://localhost/cars?First=3&After={Cursor.ToCursor(new DateTimeOffset(2000, 1, 3, 0, 0, 0, TimeSpan.Zero))}",
                previousPageUrl: null,
                expectedPageCount: 3,
                actualPageCount: 3,
                totalCount: 4,
                this.formatters
                )
            .ConfigureAwait(false);
    }
}
