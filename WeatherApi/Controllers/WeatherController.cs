using api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeatherApi.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly IRateLimitService _rateLimitService;
    private readonly ILocationValidationService _locationValidationService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(
        IWeatherService weatherService,
        IRateLimitService rateLimitService,
        ILocationValidationService locationValidationService,
        ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _rateLimitService = rateLimitService;
        _locationValidationService = locationValidationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetWeather([FromQuery] string city, [FromQuery] string country)
    {
        _logger.LogInformation("Received request to get weather for {City}, {Country}", city, country);

        // Validate location input
        if (!_locationValidationService.ValidateLocation(city, country, out var errorMessage))
        {
            _logger.LogWarning("Location validation failed for {City}, {Country}: {ErrorMessage}", city, country, errorMessage);
            return BadRequest(new { error = errorMessage });
        }

        // Check rate limit
        if (!_rateLimitService.TryConsumeToken())
        {
            _logger.LogWarning("Rate limit exceeded for weather request");
            return StatusCode(429, new { error = "Rate limit exceeded. Please try again later." });
        }

        // Fetch weather data
        var weatherResponse = await _weatherService.GetWeatherDescriptionAsync(city, country);

        // Handle potential errors from weather service
        if (!string.IsNullOrEmpty(weatherResponse.ErrorMessage))
        {
            _logger.LogError("Error retrieving weather data: {ErrorMessage}", weatherResponse.ErrorMessage);
            return StatusCode(500, new { error = weatherResponse.ErrorMessage });
        }

        _logger.LogInformation("Successfully retrieved weather for {City}, {Country}: {Description}", city, country, weatherResponse.Description);
        return Ok(new { description = weatherResponse.Description });
    }
}
