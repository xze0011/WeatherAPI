using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly List<string> _openWeatherMapApiKey;
    private readonly IRateLimitService _rateLimitService;

    public WeatherController(HttpClient httpClient, IRateLimitService rateLimitService, OpenWeatherMapApiKeys openWeatherMapApiKey)
    {
        _httpClient = httpClient;
        _rateLimitService = rateLimitService;
        _openWeatherMapApiKey = openWeatherMapApiKey.ApiKeys;
    }

    [HttpGet("getWeather")]
    public async Task<IActionResult> GetWeather([FromQuery] string city, [FromQuery] string country)
    {
        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(country))
        {
            return BadRequest("City and country must be provided.");
        }

        if (!_rateLimitService.TryConsumeToken())
        {
            return StatusCode(429, "Rate limit exceeded. Please try again later.");
        }

        var random = new Random();
        var selectedApiKey = _openWeatherMapApiKey[random.Next(_openWeatherMapApiKey.Count)];
        var requestUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city},{country}&appid={selectedApiKey}";

        try
        {

            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseBody);

            var description = json["weather"]?[0]?["description"]?.ToString();

            return Ok(new { description });

        }
        catch (HttpRequestException ex)
        {

            return StatusCode(500, $"Error retrieving data from OpenWeatherMap API: {ex.Message}");
        }
    }
}
