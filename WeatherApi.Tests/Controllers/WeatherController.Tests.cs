//using api.Models;
//using api.Interfaces;
//using FakeItEasy;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;
//using Newtonsoft.Json.Linq;

//namespace api.Tests.Controllers
//{
//    public class WeatherControllerTests
//    {
//        private readonly IRateLimitService _rateLimitService;
//        private readonly HttpClient _httpClient;
//        private readonly WeatherController _controller;
//        private readonly OpenWeatherMapApiKeys _apiKeys;

//        public WeatherControllerTests()
//        {
//            // 创建假对象
//            _rateLimitService = A.Fake<IRateLimitService>();
//            _apiKeys = new OpenWeatherMapApiKeys(new List<string>
//        {
//            "TestApiKey1",
//            "TestApiKey2"
//        });
//            // 模拟 HttpClient，使用 FakeItEasy 的 A.Fake<HttpMessageHandler>()
//            var handlerMock = A.Fake<HttpMessageHandler>();
//            _httpClient = new HttpClient(handlerMock);

//            _controller = new WeatherController(_httpClient, _rateLimitService, _apiKeys);
//        }

//        [Fact]
//        public async Task GetWeather_ShouldReturnBadRequest_WhenCityOrCountryIsMissing()
//        {
//            // Act
//            var result = await _controller.GetWeather("", "US");

//            // Assert
//            Assert.IsType<BadRequestObjectResult>(result);
//        }

//        [Fact]
//        public async Task GetWeather_ShouldReturnTooManyRequests_WhenRateLimitExceeded()
//        {
//            // Arrange
//            A.CallTo(() => _rateLimitService.TryConsumeToken()).Returns(false);

//            // Act
//            var result = await _controller.GetWeather("New York", "US");

//            // Assert
//            var statusCodeResult = Assert.IsType<ObjectResult>(result);
//            Assert.Equal(429, statusCodeResult.StatusCode);
//        }

//        [Fact]
//        public async Task GetWeather_ShouldReturnWeatherDescription_WhenApiResponseIsSuccessful()
//        {
//            // Arrange
//            A.CallTo(() => _rateLimitService.TryConsumeToken()).Returns(true);

//            // 模拟 HttpClient 返回成功的响应
//            var handlerMock = A.Fake<HttpMessageHandler>();
//            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
//            {
//                Content = new StringContent("{\"weather\": [{\"description\": \"clear sky\"}]}")
//            };

//            A.CallTo(handlerMock)
//                .Where(call => call.Method.Name == "SendAsync")
//                .WithReturnType<Task<HttpResponseMessage>>()
//                .Returns(Task.FromResult(responseMessage));

//            var httpClient = new HttpClient(handlerMock);
//            var controller = new WeatherController(httpClient, _rateLimitService, _apiKeys);

//            // Act
//            var result = await controller.GetWeather("New York", "US");

//            // Assert
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            var data = (okResult.Value as JObject);
//            Assert.Equal("clear sky", data["description"]?.ToString());
//        }

//        [Fact]
//        public async Task GetWeather_ShouldReturnInternalServerError_WhenApiRequestFails()
//        {
//            // Arrange
//            A.CallTo(() => _rateLimitService.TryConsumeToken()).Returns(true);

//            // 模拟 HttpClient 返回失败的响应
//            var handlerMock = A.Fake<HttpMessageHandler>();

//            A.CallTo(handlerMock)
//                .Where(call => call.Method.Name == "SendAsync")
//                .WithReturnType<Task<HttpResponseMessage>>()
//                .Throws(new HttpRequestException("API Error"));

//            var httpClient = new HttpClient(handlerMock);
//            var controller = new WeatherController(httpClient, _rateLimitService, _apiKeys);

//            // Act
//            var result = await controller.GetWeather("New York", "US");

//            // Assert
//            var statusCodeResult = Assert.IsType<ObjectResult>(result);
//            Assert.Equal(500, statusCodeResult.StatusCode);
//        }
//    }
//}
