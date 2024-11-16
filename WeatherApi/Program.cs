using api.Interfaces;
using api.Models;
using api.Services;
using Microsoft.Extensions.Options;
using WeatherApi.Interfaces;
using WeatherApi.Models;
using WeatherApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure options from appsettings sections
builder.Services.Configure<OpenWeatherMapConfig>(builder.Configuration.GetSection("OpenWeatherMap"));
builder.Services.Configure<RateLimitConfig>(builder.Configuration.GetSection("RateLimitConfig"));

// Register services
builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IOptions<RateLimitConfig>>().Value;
    return new TokenBucket(config.ClientKeys, config.TokenCapacity);
});
builder.Services.AddSingleton<IRateLimitService, RateLimitService>();

builder.Services.AddTransient<IWeatherService, WeatherService>();
builder.Services.AddTransient<ILocationValidationService, LocationValidationService>();

// Configure HttpClient and other MVC-related services
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
