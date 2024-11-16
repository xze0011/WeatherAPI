using WeatherApi.Models;

namespace WeatherApi.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherResponse> GetWeatherDescriptionAsync(string city, string country);
    }
}
