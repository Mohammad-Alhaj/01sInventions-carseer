using System.Text.Json;
using inventions_carseer.Models;
using inventions_carseer.Services.Nhtsa;
using Microsoft.Extensions.Caching.Memory;

namespace inventions_carseer.Services;

public class NhtsaVehicleService(HttpClient httpClient, IMemoryCache cache, ILogger<NhtsaVehicleService> logger)
    : IVehicleService
{
    private const string MakesCacheKey = "nhtsa:makes";
    private static readonly TimeSpan MakesCacheDuration = TimeSpan.FromHours(12);

    public async Task<IReadOnlyList<VehicleMake>> GetMakesAsync(CancellationToken cancellationToken = default)
    {
        // Get from Memory 
        if (cache.TryGetValue(MakesCacheKey, out IReadOnlyList<VehicleMake>? cached) && cached is not null)
        {
            return cached;
        }

        var results = await GetResultsAsync<NhtsaMake>("getallmakes?format=json", cancellationToken);

        // All those operations on Memory , In real case should be handle by db
        IReadOnlyList<VehicleMake> makes = results
            .Where(m => !string.IsNullOrWhiteSpace(m.MakeName))
            .Select(m => new VehicleMake(m.MakeId, m.MakeName!.Trim()))
            .OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (makes.Count > 0)
        {
            cache.Set(MakesCacheKey, makes, MakesCacheDuration);
        }

        return makes;
    }

    public async Task<IReadOnlyList<VehicleType>> GetVehicleTypesAsync(
        int makeId, CancellationToken cancellationToken = default)
    {
        var results = await GetResultsAsync<NhtsaVehicleType>(
            $"GetVehicleTypesForMakeId/{makeId}?format=json", cancellationToken);

        return results
            .Where(t => !string.IsNullOrWhiteSpace(t.VehicleTypeName))
            .Select(t => new VehicleType(t.VehicleTypeId, t.VehicleTypeName!.Trim()))
            .OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<IReadOnlyList<VehicleModel>> GetModelsAsync(
        int makeId, int year, string? vehicleType, CancellationToken cancellationToken = default)
    {
        var url = $"GetModelsForMakeIdYear/makeId/{makeId}/modelyear/{year}";
        if (!string.IsNullOrWhiteSpace(vehicleType))
        {
            url += $"/vehicletype/{Uri.EscapeDataString(vehicleType.Trim())}";
        }

        var results = await GetResultsAsync<NhtsaModel>(url + "?format=json", cancellationToken);

        return results
            .Where(m => !string.IsNullOrWhiteSpace(m.ModelName))
            .Select(m => new VehicleModel(m.ModelId, m.ModelName!.Trim(), m.VehicleTypeName))
            .OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private async Task<IReadOnlyList<T>> GetResultsAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<NhtsaResponse<T>>(relativeUrl, cancellationToken);
            return response?.Results ?? [];
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(ex, "NHTSA API request timed out: {Url}", relativeUrl);
            throw new VehicleServiceException(
                "The vehicle data service took too long to respond. Please try again.", ex);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "NHTSA API request failed: {Url}", relativeUrl);
            throw new VehicleServiceException(
                "The vehicle data service is currently unavailable. Please try again later.", ex);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "NHTSA API returned an unexpected response: {Url}", relativeUrl);
            throw new VehicleServiceException("The vehicle data service returned an unexpected response.", ex);
        }
    }
}
