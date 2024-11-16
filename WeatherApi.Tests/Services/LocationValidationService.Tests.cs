using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using WeatherApi.Services;

namespace WeatherApi.Tests.Services
{
    public class LocationValidationServiceTests
    {
        private readonly LocationValidationService _service;

        public LocationValidationServiceTests()
        {
            var fakeLogger = A.Fake<ILogger<LocationValidationService>>();
            _service = new LocationValidationService(fakeLogger);
        }

        [Theory]
        [InlineData("New York", "USA", true)]
        [InlineData("Los Angeles", "United States", true)]
        [InlineData("Tokyo", "Japan", true)]
        [InlineData("Paris", "France", true)]
        public void ValidateLocation_ShouldReturnTrue_ForValidCityAndCountry(string city, string country, bool expectedResult)
        {
            // Act
            var result = _service.ValidateLocation(city, country, out string? errorMessage);

            // Assert
            result.Should().Be(expectedResult);
            errorMessage.Should().BeNull();
        }

        [Theory]
        [InlineData(null, "USA", "City and country must be provided.")]
        [InlineData("Los Angeles", null, "City and country must be provided.")]
        [InlineData("", "France", "City and country must be provided.")]
        [InlineData("Paris", "", "City and country must be provided.")]
        public void ValidateLocation_ShouldReturnFalse_WhenCityOrCountryIsNullOrEmpty(string city, string country, string expectedErrorMessage)
        {
            // Act
            var result = _service.ValidateLocation(city, country, out string? errorMessage);

            // Assert
            result.Should().BeFalse();
            errorMessage.Should().Be(expectedErrorMessage);
        }

        [Theory]
        [InlineData("New York1", "USA", "City must contain only letters and spaces.")]
        [InlineData("Los Angeles", "United States1", "Country must contain only letters and spaces.")]
        [InlineData("Tokyo@", "Japan", "City must contain only letters and spaces.")]
        [InlineData("Paris", "Fran$ce", "Country must contain only letters and spaces.")]
        public void ValidateLocation_ShouldReturnFalse_WhenCityOrCountryContainsInvalidCharacters(string city, string country, string expectedErrorMessage)
        {
            // Act
            var result = _service.ValidateLocation(city, country, out string? errorMessage);

            // Assert
            result.Should().BeFalse();
            errorMessage.Should().Be(expectedErrorMessage);
        }
    }
}
