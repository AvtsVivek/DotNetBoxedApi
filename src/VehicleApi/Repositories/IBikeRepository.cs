namespace VehicleApi.Repositories;

using VehicleApi.Models;

public interface IBikeRepository
{
    Task<Bike> AddAsync(Bike bike, CancellationToken cancellationToken);

    Task DeleteAsync(Bike bike, CancellationToken cancellationToken);

    Task<Bike?> GetAsync(int bikeId, CancellationToken cancellationToken);
}
