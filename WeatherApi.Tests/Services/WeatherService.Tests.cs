using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using WeatherApi.Interfaces;
using WeatherApi.Models;
using WeatherApi.Services;
using Xunit;

namespace WeatherApi.Tests.Services
{
    public class WeatherServiceTests
    {
        private readonly IOptions<OpenWeatherMapConfig> _fakeConfig;
        private readonly ILogger<WeatherService> _fakeLogger;
        private readonly HttpClient _httpClient;
        private readonly HttpMessageHandler _fakeHttpHandler;
        private readonly WeatherService _weatherService;

        public WeatherServiceTests()
        {
            // Set up fake configuration with base URL and API keys
            _fakeConfig = Options.Create(new OpenWeatherMapConfig
            {
                BaseUrl = "https://fakeapi/weather",
                ApiKeys = new List<string> { "fakeApiKey1", "fakeApiKey2" }
            });

            // Create a fake logger
            _fakeLogger = A.Fake<ILogger<WeatherService>>();

            // Set up a fake HTTP handler to simulate HTTP responses
            _fakeHttpHandler = A.Fake<HttpMessageHandler>();
            _httpClient = new HttpClient(_fakeHttpHandler) { BaseAddress = new Uri(_fakeConfig.Value.BaseUrl) };

            // Initialize the WeatherService with fake dependencies
            _weatherService = new WeatherService(_httpClient, _fakeConfig, _fakeLogger);
        }

        [Fact]
        public async Task GetWeatherDescriptionAsync_ShouldReturnError_WhenBaseUrlIsMissing()
        {
            // Arrange
            var configWithoutBaseUrl = Options.Create(new OpenWeatherMapConfig
            {
                BaseUrl = "",
                ApiKeys = new List<string> { "fakeApiKey1" }
            });
            var weatherService = new WeatherService(_httpClient, configWithoutBaseUrl, _fakeLogger);

            // Act
            var result = await weatherService.GetWeatherDescriptionAsync("New York", "USA");

            // Assert
            result.ErrorMessage.Should().Be("Configuration error: Base URL for OpenWeatherMap API is missing.");
        }

        [Fact]
        public async Task GetWeatherDescriptionAsync_ShouldReturnError_WhenNoApiKeysAreAvailable()
        {
            // Arrange
            var configWithoutApiKeys = Options.Create(new OpenWeatherMapConfig
            {
                BaseUrl = "https://fakeapi/weather",
                ApiKeys = new List<string>()
            });
            var weatherService = new WeatherService(_httpClient, configWithoutApiKeys, _fakeLogger);

            // Act
            var result = await weatherService.GetWeatherDescriptionAsync("New York", "USA");

            // Assert
            result.ErrorMessage.Should().Be("Configuration error: No valid API keys available for OpenWeatherMap.");
        }

        [Fact]
        public async Task GetWeatherDescriptionAsync_ShouldReturnDescription_WhenApiResponseIsSuccessful()
        {
            // Arrange
            var responseContent = new JObject
            {
                ["weather"] = new JArray
                {
                    new JObject
                    {
                        ["description"] = "clear sky"
                    }
                }
            }.ToString();

            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            };

            A.CallTo(_fakeHttpHandler)
                .Where(call => call.Method.Name == "SendAsync")
                .WithReturnType<Task<HttpResponseMessage>>()
                .Returns(Task.FromResult(fakeResponse));

            // Act
            var result = await _weatherService.GetWeatherDescriptionAsync("New York", "USA");

            // Assert
            result.Description.Should().Be("clear sky");
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task GetWeatherDescriptionAsync_ShouldReturnError_WhenApiResponseHasNoDescription()
        {
            // Arrange
            var responseContent = new JObject
            {
                ["weather"] = new JArray
                {
                    new JObject{}
                }
            }.ToString();

            var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            };

            A.CallTo(_fakeHttpHandler)
                .Where(call => call.Method.Name == "SendAsync")
                .WithReturnType<Task<HttpResponseMessage>>()
                .Returns(Task.FromResult(fakeResponse));

            // Act
            var result = await _weatherService.GetWeatherDescriptionAsync("New York", "USA");

            // Assert
            result.ErrorMessage.Should().Be("Weather description not available from the API.");
        }

        [Fact]
        public async Task GetWeatherDescriptionAsync_ShouldReturnError_WhenHttpRequestFails()
        {
            // Arrange
            var fakeException = new HttpRequestException("Network error");
            A.CallTo(_fakeHttpHandler)
                .Where(call => call.Method.Name == "SendAsync")
                .WithReturnType<Task<HttpResponseMessage>>()
                .Throws(fakeException);

            // Act
            var result = await _weatherService.GetWeatherDescriptionAsync("New York", "USA");

            // Assert
            result.ErrorMessage.Should().Be("Error retrieving data from API: Network error");
        }
    }
}
