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
    private readonly WeatherForecastMapper _weatherForecastMapper;
    private readonly ForecastRepository _forecastRepository;

    public ForecastService(ILogger<WeatherForecastController> logger,
                           HttpClient httpClient,
                           WeatherForecastMapper weatherForecastMapper,
                           ForecastRepository forecastRepository)
    {
        _logger = logger;
        _httpClient = httpClient;
        _weatherForecastMapper = weatherForecastMapper;
        _forecastRepository = forecastRepository;
    }

    public async Task<WeatherForecast?> GetForecast()
    {
        try
        {
            WeatherForecastDto dto = await FetchForecastsFromExtrnalServiceWithTimeout();
            WeatherForecast weatherForecast = _weatherForecastMapper.MapWeatherForecastDtoToWeatherForecast(dto);
            await _forecastRepository.Insert(weatherForecast);
            return weatherForecast;
        }
        catch (TaskCanceledException)
        {
            return await _forecastRepository.FetchLastForecast();
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

    private async Task<WeatherForecastDto> FetchForecastsFromExtrnalServiceWithTimeout()
    {
        using var cancellationTokenSource = new CancellationTokenSource(ApplicationSettings.DefaultTimeout);

        const string forecastRoute = "forecast?latitude=52.52&longitude=13.41&hourly=temperature_2m,relativehumidity_2m,windspeed_10m";


        HttpResponseMessage responseMessage = await _httpClient.GetAsync(forecastRoute, cancellationTokenSource.Token);

        responseMessage.EnsureSuccessStatusCode();
        Stream responseBody = await responseMessage.Content.ReadAsStreamAsync();

        return (await JsonSerializer.DeserializeAsync<WeatherForecastDto>(responseBody, _jsonSerializerOptions))!;
    }
}
