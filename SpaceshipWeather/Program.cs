
using SpaceshipWeather;
using SpaceshipWeather.BackgroundServices;
using SpaceshipWeather.Models;
using SpaceshipWeather.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Services
builder.Services.AddScoped<ForecastService>();
builder.Services.AddSingleton<WeatherForecastMapper>();
builder.Services.AddSingleton<DatabaseInitilizer>();
builder.Services.AddSingleton<ForecastRepository>();
builder.Services.AddHostedService<DatabaseCleanupService>();

builder.Services.AddHttpClient<ForecastService>(client =>
{
    client.BaseAddress = new Uri(ApplicationSettings.BaseAddress);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.Services.GetRequiredService<DatabaseInitilizer>().Setup();

app.Run();
