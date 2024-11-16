namespace WeatherApi.Interfaces
{
    public interface ILocationValidationService
    {
        bool ValidateLocation(string city, string country, out string? errorMessage);
    }
}
