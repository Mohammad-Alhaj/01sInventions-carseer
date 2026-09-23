namespace inventions_carseer.Models;

public record VehicleMake(int Id, string Name);

public record VehicleType(int Id, string Name);

public record VehicleModel(int Id, string Name, string? VehicleTypeName);
