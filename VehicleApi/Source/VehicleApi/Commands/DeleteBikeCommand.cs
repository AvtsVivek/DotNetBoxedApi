namespace VehicleApi.Commands;

using VehicleApi.Repositories;
using Microsoft.AspNetCore.Mvc;

public class DeleteBikeCommand
{
    // As of now the bike repository is not implimented.
    // So Just use Car repository.
    private readonly ICarRepository carRepository;

    public DeleteBikeCommand(ICarRepository carRepository) =>
        this.carRepository = carRepository;

    public async Task<IActionResult> ExecuteAsync(int bikeId, CancellationToken cancellationToken)
    {
        var bike = await this.carRepository.GetAsync(bikeId, cancellationToken).ConfigureAwait(false);

        if (bike is null)
        {
            return new NotFoundResult();
        }

        await this.carRepository.DeleteAsync(bike, cancellationToken).ConfigureAwait(false);

        return new NoContentResult();
    }
}
