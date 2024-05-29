using SpaceshipWeather.Controllers;
using SpaceshipWeather.Models;
using SpaceshipWeather.Models.Dtos;
using SpaceshipWeather.Models.Entities;
using System.Text.Json;

namespace SpaceshipWeather.Services;

public class ForecastService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly HttpClient _httpClient;
    private readonly WeatherForcastMapper _weatherForcastMapper;

    public ForecastService(ILogger<WeatherForecastController> logger, HttpClient httpClient, WeatherForcastMapper weatherForcastMapper)
    {
        _logger = logger;
        _httpClient = httpClient;
        _weatherForcastMapper = weatherForcastMapper;
    }

    public async Task<WeatherForcast?> GetForcast()
    {
        try
        {
            WeatherForcastDto dto = await FetchForcastsFromExtrnalServiceWithTimeout();
            return _weatherForcastMapper.MapWeatherForcastDtoToWeatherForcast(dto);
        }
        catch (TaskCanceledException)
        {
            return await FetchLastForcastsFromDatabase();
        }
        catch (Exception e)
        {
            _logger.LogError($"""
                Something went wrong
                Message: {e.Message}
                Stack Trace: {e.StackTrace}
                """);
            return null;
        }
    }

    private async Task<WeatherForcastDto> FetchForcastsFromExtrnalServiceWithTimeout()
    {
        using var cancellationTokenSource = new CancellationTokenSource(ApplicationSettings.DefaultTimeout);

        const string forecastRoute = "forecast?latitude=52.52&longitude=13.41&hourly=temperature_2m,relativehumidity_2m,windspeed_10m";


        HttpResponseMessage responseMessage = await _httpClient.GetAsync(forecastRoute, cancellationTokenSource.Token);

        responseMessage.EnsureSuccessStatusCode();
        Stream responseBody = await responseMessage.Content.ReadAsStreamAsync();

        return (await JsonSerializer.DeserializeAsync<WeatherForcastDto>(responseBody, _jsonSerializerOptions))!;
    }


    private async Task<WeatherForcast> FetchLastForcastsFromDatabase() => throw new NotImplementedException();
}
