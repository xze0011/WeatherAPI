using Newtonsoft.Json.Linq;
using WeatherApi.Models;
using WeatherApi.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace WeatherApi.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly List<string> _apiKeys;
        private readonly string _baseUrl;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(HttpClient httpClient, IOptions<OpenWeatherMapConfig> openWeatherMapConfig, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            var config = openWeatherMapConfig.Value;
            _apiKeys = config.ApiKeys;
            _baseUrl = config.BaseUrl;
            _logger = logger;
        }

        public async Task<WeatherResponse> GetWeatherDescriptionAsync(string city, string country)
        {
            // Log start of weather request
            _logger.LogInformation("Fetching weather data for {City}, {Country}", city, country);

            // Check configuration
            if (string.IsNullOrEmpty(_baseUrl))
            {
                _logger.LogError("Configuration error: Base URL for OpenWeatherMap API is missing.");
                return new WeatherResponse
                {
                    ErrorMessage = "Configuration error: Base URL for OpenWeatherMap API is missing."
                };
            }

            if (_apiKeys == null || !_apiKeys.Any() || _apiKeys.Any(string.IsNullOrEmpty))
            {
                _logger.LogError("Configuration error: No valid API keys available for OpenWeatherMap.");
                return new WeatherResponse
                {
                    ErrorMessage = "Configuration error: No valid API keys available for OpenWeatherMap."
                };
            }

            // Select a random API key
            var random = new Random();
            var selectedApiKey = _apiKeys[random.Next(_apiKeys.Count)];
            var requestUrl = $"{_baseUrl}?q={city},{country}&appid={selectedApiKey}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);

                var description = json["weather"]?[0]?["description"]?.ToString();

                if (string.IsNullOrEmpty(description))
                {
                    _logger.LogWarning("Weather description not available from the API for {City}, {Country}", city, country);
                    return new WeatherResponse
                    {
                        ErrorMessage = "Weather description not available from the API."
                    };
                }

                _logger.LogInformation("Successfully retrieved weather description for {City}, {Country}: {Description}", city, country, description);
                return new WeatherResponse
                {
                    Description = description
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error retrieving data from OpenWeatherMap API for {City}, {Country}", city, country);
                return new WeatherResponse
                {
                    ErrorMessage = $"Error retrieving data from API: {ex.Message}"
                };
            }
        }
    }
}
