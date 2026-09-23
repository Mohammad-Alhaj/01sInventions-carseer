using System.ComponentModel.DataAnnotations;

namespace inventions_carseer.Models;

public class VehicleSearchViewModel
{
    public const int MinYear = 1800;
    public static int MaxYear => DateTime.UtcNow.Year + 1;

    [Required(ErrorMessage = "Please select a make.")]
    [Display(Name = "Make")]
    public int? MakeId { get; set; }

    [Required(ErrorMessage = "Please enter a model year.")]
    [Display(Name = "Model year")]
    public int? Year { get; set; }

    [Display(Name = "Vehicle type")]
    [StringLength(100)]
    public string? VehicleType { get; set; }

    public IReadOnlyList<VehicleMake> Makes { get; set; } = [];
    public IReadOnlyList<VehicleType> VehicleTypes { get; set; } = [];
    public IReadOnlyList<VehicleModel>? Models { get; set; }
    public string? ErrorMessage { get; set; }

    public string? SelectedMakeName => Makes.FirstOrDefault(m => m.Id == MakeId)?.Name;


}
