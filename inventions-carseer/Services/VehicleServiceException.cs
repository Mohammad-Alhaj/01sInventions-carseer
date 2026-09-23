namespace inventions_carseer.Services;

public class VehicleServiceException(string message, Exception? innerException = null)
    : Exception(message, innerException);
