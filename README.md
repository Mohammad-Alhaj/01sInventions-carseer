# Vehicle Model Search

A small ASP.NET Core MVC app for looking up vehicle models with the [NHTSA vPIC API](https://vpic.nhtsa.dot.gov/api/).
Pick a make, enter a model year, and optionally choose a vehicle type. The app then lists the matching models.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Internet access, because the app calls `vpic.nhtsa.dot.gov` live

## Run locally

```bash
dotnet run --project inventions-carseer
```

Open the URL printed in the console (by default `https://localhost:7xxx` or `http://localhost:5xxx`, set in `inventions-carseer/Properties/launchSettings.json`).

You can also open `inventions-carseer.sln` in Visual Studio or Rider and run the `inventions-carseer` project.

## Configuration

`inventions-carseer/appsettings.json`:

```json
"NhtsaApi": {
  "BaseUrl": "https://vpic.nhtsa.dot.gov/api/vehicles/",
  "TimeoutSeconds": 15
}
```

Both values are validated when the app starts. You can override them with environment variables, e.g. `NhtsaApi__TimeoutSeconds=30`.

## Architecture

```
VehiclesController  ->  IVehicleService  ->  NhtsaVehicleService  ->  HttpClient (IHttpClientFactory)  ->  NHTSA vPIC API
```

- `Controllers/VehiclesController.cs` is a thin controller. `Index` shows the form, `Search` runs the search, and `VehicleTypes` returns JSON that the type dropdown loads when the make changes.
- `Services/NhtsaVehicleService.cs` makes all the API calls:
  - It maps the NHTSA JSON (`Services/Nhtsa/NhtsaDtos.cs`) to simple app models (`Models/VehicleRecords.cs`).
  - It caches the large makes list (~12k entries) in memory for 12 hours.
  - It turns network failures, timeouts and invalid JSON into a `VehicleServiceException` with a user-friendly message, which the controller shows as an alert.
- `Models/VehicleSearchViewModel.cs` holds the form input and the results. It also validates input: a make is required, and the year must be between 1995 and next year.

APIs used:

| Purpose | Endpoint |
|---|---|
| All makes | `getallmakes?format=json` |
| Vehicle types for a make | `GetVehicleTypesForMakeId/{makeId}?format=json` |
| Models for make + year | `GetModelsForMakeIdYear/makeId/{makeId}/modelyear/{year}[/vehicletype/{type}]?format=json` |

## Run with Docker

```bash
docker build -t inventions-carseer .
docker run -d \
  --name carseer \
  --restart unless-stopped \
  -p 8080:8080 \
  nventions-carseer
```

Open `http://localhost:8080`.

