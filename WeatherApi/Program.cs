using api.Interfaces;
using api.Models;
using api.Services;
var builder = WebApplication.CreateBuilder(args);

var openWeatherMapApiKeys = new List<string>
{
    builder.Configuration["OpenWeatherMapApiKey1"],
    builder.Configuration["OpenWeatherMapApiKey2"]
};
builder.Services.AddSingleton(new OpenWeatherMapApiKeys(openWeatherMapApiKeys));

// Add Client API KEY 
var clientKeys = new List<string>();
for (int i = 1; i <= 5; i++)
{
    string clientKey = builder.Configuration[$"ClientKey{i}"];
    if (!string.IsNullOrEmpty(clientKey))
    {
        clientKeys.Add(clientKey);
    }
}
var tokenBucket = new TokenBucket(clientKeys, 2);

// 注册 TokenBucket 为单例
builder.Services.AddSingleton(tokenBucket);

// 注册 RateLimitService，传入 TokenBucket 和容量
builder.Services.AddSingleton<IRateLimitService>(new RateLimitService(tokenBucket));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.MapControllers();


app.Run();
