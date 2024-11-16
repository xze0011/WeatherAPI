using System.Text.RegularExpressions;
using WeatherApi.Interfaces;
using Microsoft.Extensions.Logging;

namespace WeatherApi.Services
{
    public class LocationValidationService : ILocationValidationService
    {
        private static readonly Regex ValidLocationRegex = new Regex("^[a-zA-Z ]+$", RegexOptions.Compiled);
        private readonly ILogger<LocationValidationService> _logger;

        public LocationValidationService(ILogger<LocationValidationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Validates city and country names, allowing only letters and spaces.
        /// </summary>
        /// <param name="city">The city name to validate.</param>
        /// <param name="country">The country name to validate.</param>
        /// <param name="errorMessage">Output error message if validation fails.</param>
        /// <returns>True if both city and country are valid; otherwise, false.</returns>
        public bool ValidateLocation(string city, string country, out string? errorMessage)
        {
            if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(country))
            {
                errorMessage = "City and country must be provided.";
                _logger.LogWarning("Validation failed: {ErrorMessage}", errorMessage);
                return false;
            }

            if (!ValidLocationRegex.IsMatch(city))
            {
                errorMessage = "City must contain only letters and spaces.";
                _logger.LogWarning("Validation failed for city '{City}': {ErrorMessage}", city, errorMessage);
                return false;
            }

            if (!ValidLocationRegex.IsMatch(country))
            {
                errorMessage = "Country must contain only letters and spaces.";
                _logger.LogWarning("Validation failed for country '{Country}': {ErrorMessage}", country, errorMessage);
                return false;
            }

            errorMessage = null;
            _logger.LogInformation("Location validation succeeded for {City}, {Country}", city, country);
            return true;
        }
    }
}
