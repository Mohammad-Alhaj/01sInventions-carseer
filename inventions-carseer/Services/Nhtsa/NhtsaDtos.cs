using System.Text.Json.Serialization;

namespace inventions_carseer.Services.Nhtsa;

internal sealed class NhtsaResponse<T>
{
    public int Count { get; set; }
    public string? Message { get; set; }
    public List<T>? Results { get; set; }
}

internal sealed class NhtsaMake
{
    [JsonPropertyName("Make_ID")]
    public int MakeId { get; set; }

    [JsonPropertyName("Make_Name")]
    public string? MakeName { get; set; }
}

internal sealed class NhtsaVehicleType
{
    [JsonPropertyName("VehicleTypeId")]
    public int VehicleTypeId { get; set; }

    [JsonPropertyName("VehicleTypeName")]
    public string? VehicleTypeName { get; set; }
}

internal sealed class NhtsaModel
{
    [JsonPropertyName("Model_ID")]
    public int ModelId { get; set; }

    [JsonPropertyName("Model_Name")]
    public string? ModelName { get; set; }

    [JsonPropertyName("VehicleTypeName")]
    public string? VehicleTypeName { get; set; }
}
