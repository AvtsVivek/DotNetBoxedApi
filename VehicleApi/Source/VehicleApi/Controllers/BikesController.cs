namespace VehicleApi.Controllers;

using VehicleApi.Commands;
using VehicleApi.Constants;
using VehicleApi.Services;
using VehicleApi.ViewModels;
using Boxed.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Route("[controller]")]
[ApiController]
[ApiVersion(ApiVersionName.V1)]
[SwaggerResponse(
    StatusCodes.Status500InternalServerError,
    "The MIME type in the Accept HTTP header is not acceptable.",
    typeof(ProblemDetails),
    ContentType.ProblemJson)]
public class BikesController : ControllerBase
{

    public BikesController()
    {

    }

    /// <summary>
    /// Deletes the bike with the specified unique identifier.
    /// </summary>
    /// <param name="command">The action command.</param>
    /// <param name="bikeId">The cars unique identifier.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the HTTP request.</param>
    /// <returns>A 204 No Content response if the bike was deleted or a 404 Not Found if a bike with the specified
    /// unique identifier was not found.</returns>
    [HttpDelete("{bikeId}", Name = BikesControllerRoute.DeleteBike)]
    [SwaggerResponse(StatusCodes.Status204NoContent, "The bike with the specified unique identifier was deleted.")]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "A bike with the specified unique identifier was not found.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    public Task<IActionResult> DeleteAsync(
        [FromServices] DeleteBikeCommand command,
        int bikeId,
        CancellationToken cancellationToken) => command.ExecuteAsync(bikeId, cancellationToken);


    /// <summary>
    /// Gets the bike with the specified unique identifier.
    /// </summary>
    /// <param name="command">The action command.</param>
    /// <param name="bikeId">The bikes unique identifier.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the HTTP request.</param>
    /// <returns>A 200 OK response containing the bike or a 404 Not Found if a car with the specified unique
    /// identifier was not found.</returns>
    [HttpGet("{bikeId}", Name = BikesControllerRoute.GetBike)]
    [HttpHead("{bikeId}", Name = BikesControllerRoute.HeadBike)]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "The bike with the specified unique identifier.",
        typeof(Bike),
        ContentType.RestfulJson,
        ContentType.Json)]
    [SwaggerResponse(
        StatusCodes.Status304NotModified,
        "The bike has not changed since the date given in the If-Modified-Since HTTP header.")]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "A bike with the specified unique identifier could not be found.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    [SwaggerResponse(
        StatusCodes.Status406NotAcceptable,
        "The MIME type in the Accept HTTP header is not acceptable.",
        typeof(ProblemDetails),
        ContentType.ProblemJson)]
    public Task<IActionResult> GetAsync(
        [FromServices] GetBikeCommand command,
        int bikeId,
        CancellationToken cancellationToken) => command.ExecuteAsync(bikeId, cancellationToken);


    [HttpGet("Fetch/{inputParam}", Name = "FetchSomething")]
    public IActionResult Fetch([FromServices] IGreeterService greeterService,
    string inputParam)
    {
        var greetingMessage = greeterService.Greeting(inputParam);
        var simpleObject = new { Amount = 108, Message = $"Hello {inputParam}", GreetingMessage = greetingMessage };
        return new OkObjectResult(simpleObject);
    }

}
#pragma warning restore CA1062 // Validate arguments of public methods
#pragma warning restore CA1822 // Mark members as static
