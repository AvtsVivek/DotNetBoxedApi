namespace VehicleApi.IntegrationTest;
using System.Net.Http.Formatting;
using VehicleApi.ViewModels;
using Xunit;

internal class TestUtilities
{
    internal static List<Models.Car> GetCars() => new()
    {
        new Models.Car() { Created = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero) },
        new Models.Car() { Created = new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.Zero) },
        new Models.Car() { Created = new DateTimeOffset(2000, 1, 3, 0, 0, 0, TimeSpan.Zero) },
        new Models.Car() { Created = new DateTimeOffset(2000, 1, 4, 0, 0, 0, TimeSpan.Zero) },
    };


    internal static async Task AssertPageUrlsAsync(
        HttpResponseMessage response,
        string? nextPageUrl,
        string? previousPageUrl,
        int expectedPageCount,
        int actualPageCount,
        int totalCount,
        MediaTypeFormatterCollection formatters
        )
    {
        var connection = await response.Content.ReadAsAsync<Connection<Car>>(formatters).ConfigureAwait(false);

        Assert.Equal(actualPageCount, connection.Items.Count);
        Assert.Equal(actualPageCount, connection.PageInfo.Count);
        Assert.Equal(totalCount, connection.TotalCount);

        Assert.Equal(nextPageUrl is not null, connection.PageInfo.HasNextPage);
        Assert.Equal(previousPageUrl is not null, connection.PageInfo.HasPreviousPage);

        if (nextPageUrl is null)
        {
            Assert.Null(nextPageUrl);
        }
        else
        {
            Assert.Equal(new Uri(nextPageUrl), connection.PageInfo.NextPageUrl);
        }

        if (previousPageUrl is null)
        {
            Assert.Null(previousPageUrl);
        }
        else
        {
            Assert.Equal(new Uri(previousPageUrl), connection.PageInfo.PreviousPageUrl);
        }

        var firstPageUrl = $"http://localhost/cars?First={expectedPageCount}";
        var lastPageUrl = $"http://localhost/cars?Last={expectedPageCount}";

        Assert.Equal(new Uri(firstPageUrl), connection.PageInfo.FirstPageUrl);
        Assert.Equal(new Uri(lastPageUrl), connection.PageInfo.LastPageUrl);

        var linkValue = Assert.Single(response.Headers.GetValues("Link"));
        var expectedLink = $"<{firstPageUrl}>; rel=\"first\", <{lastPageUrl}>; rel=\"last\"";
        if (previousPageUrl is not null)
        {
            expectedLink = $"<{previousPageUrl}>; rel=\"previous\", " + expectedLink;
        }

        if (nextPageUrl is not null)
        {
            expectedLink = $"<{nextPageUrl}>; rel=\"next\", " + expectedLink;
        }

        Assert.Equal(expectedLink, linkValue);
    }
}
