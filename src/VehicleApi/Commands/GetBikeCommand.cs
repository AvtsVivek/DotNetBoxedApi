namespace VehicleApi.Commands;

using System.Globalization;
using VehicleApi.ViewModels;
using VehicleApi.Repositories;
using VehicleApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Boxed.Mapping;

public class GetBikeCommand
{
    private readonly IActionContextAccessor actionContextAccessor;
    private readonly IBikeRepository bikeRepository;
    private readonly IMapper<Models.Bike, Bike> bikeMapper;
    private readonly IGreeterService greetorService;

    public GetBikeCommand(
        IActionContextAccessor actionContextAccessor,
        IBikeRepository bikeRepository,
        IMapper<Models.Bike, Bike> bikeMapper,
        IGreeterService greetorService
        )
    {
        this.actionContextAccessor = actionContextAccessor;
        this.bikeRepository = bikeRepository;
        this.bikeMapper = bikeMapper;
        this.greetorService = greetorService;
    }

    public async Task<IActionResult> ExecuteAsync(int bikeId, CancellationToken cancellationToken)
    {
        var bike = await this.bikeRepository.GetAsync(bikeId, cancellationToken).ConfigureAwait(false);
        if (bike is null)
        {
            return new NotFoundResult();
        }

        var httpContext = this.actionContextAccessor.ActionContext!.HttpContext;
        var ifModifiedSince = httpContext.Request.Headers.IfModifiedSince;
        if (ifModifiedSince.Any() && DateTimeOffset.TryParse(ifModifiedSince,
            out var ifModifiedSinceDateTime) &&
            (ifModifiedSinceDateTime >= bike.Modified))
        {
            return new StatusCodeResult(StatusCodes.Status304NotModified);
        }


        var bikeViewModel = this.bikeMapper.Map(bike);

        var greetingMessage = this.greetorService.Greeting(bikeViewModel.Model);

        var bikeWithGreeting = new BikeWithGreeting()
        {
            Bike = bikeViewModel,
            Message = greetingMessage
        };

        httpContext.Response.Headers.LastModified = bike.Modified.ToString("R", CultureInfo.InvariantCulture);
        return new OkObjectResult(bikeWithGreeting);
    }
}

public class BikeWithGreeting
{
    public Bike Bike { get; set; } = default!;
    public string Message { get; set; } = default!;
}
