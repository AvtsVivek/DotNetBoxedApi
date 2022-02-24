namespace VehicleApi.Mappers;

using VehicleApi.Constants;
using VehicleApi.ViewModels;
using Boxed.Mapping;

public class BikeToBikeMapper : IMapper<Models.Bike, Bike>
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly LinkGenerator linkGenerator;

    public BikeToBikeMapper(
        IHttpContextAccessor httpContextAccessor,
        LinkGenerator linkGenerator)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.linkGenerator = linkGenerator;
    }

    public void Map(Models.Bike source, Bike destination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        destination.BikeId = source.BikeId;
        destination.Cylinders = source.Cylinders;
        destination.Make = source.Make;
        destination.Model = source.Model;
        destination.Url = new Uri(this.linkGenerator.GetUriByRouteValues(
            this.httpContextAccessor.HttpContext!,
            BikesControllerRoute.GetBike,

            new { source.BikeId })!);
    }
}
