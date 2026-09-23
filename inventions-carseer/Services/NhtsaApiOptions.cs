using System.ComponentModel.DataAnnotations;

namespace inventions_carseer.Services;

public class NhtsaApiOptions
{
    public const string SectionName = "NhtsaApi";
    
    public string BaseUrl { get; set; } = "https://vpic.nhtsa.dot.gov/api/vehicles/";
    
    public int TimeoutSeconds { get; set; } = 15;
}
