namespace VehicleApi.ViewModels;

/// <summary>
/// A make and model of Bike.
/// </summary>
public class Bike
{
    /// <summary>
    /// Gets or sets the bikes unique identifier.
    /// </summary>
    /// <example>1</example>
    public int BikeId { get; set; }

    /// <summary>
    /// Gets or sets the number of cylinders in the bikes engine.
    /// </summary>
    /// <example>2</example>
    public int Cylinders { get; set; }

    /// <summary>
    /// Gets or sets the make of the bike.
    /// </summary>
    /// <example>Honda</example>
    public string Make { get; set; } = default!;

    /// <summary>
    /// Gets or sets the model of the bike.
    /// </summary>
    /// <example>Activa</example>
    public string Model { get; set; } = default!;

    /// <summary>
    /// Gets or sets the URL used to retrieve the resource conforming to REST'ful JSON http://restfuljson.org/.
    /// </summary>
    /// <example>/bikes/1</example>
    public Uri Url { get; set; } = default!;
}
