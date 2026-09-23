using inventions_carseer.Models;
using inventions_carseer.Services;
using Microsoft.AspNetCore.Mvc;

namespace inventions_carseer.Controllers;

public class VehiclesController(IVehicleService vehicleService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new VehicleSearchViewModel();
         model.Makes = await vehicleService.GetMakesAsync(cancellationToken);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Search(VehicleSearchViewModel model, CancellationToken cancellationToken)
    {
        model.Makes = await vehicleService.GetMakesAsync(cancellationToken);
        if (model.ErrorMessage is not null)
        {
            return View(nameof(Index), model);
        }

        try
        {
            if (model.MakeId != null)
            {
                model.VehicleTypes = await vehicleService.GetVehicleTypesAsync(model.MakeId.Value, cancellationToken);


                if (ModelState.IsValid)
                {
                    model.Models = await vehicleService.GetModelsAsync(
                        model.MakeId!.Value, model.Year!.Value, model.VehicleType, cancellationToken);
                }
            }
        }
        catch (VehicleServiceException ex)
        {
            model.ErrorMessage = ex.Message;
        }

        return View(nameof(Index), model);
    }

    [HttpGet]
    public async Task<IActionResult> VehicleTypes(int makeId, CancellationToken cancellationToken)
    {
        if (makeId <= 0)
        {
            return BadRequest(new { error = "A valid make is required." });
        }

        try
        {
            var types = await vehicleService.GetVehicleTypesAsync(makeId, cancellationToken);
            return Json(types);
        }
        catch (VehicleServiceException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { error = ex.Message });
        }
    }
    
}
