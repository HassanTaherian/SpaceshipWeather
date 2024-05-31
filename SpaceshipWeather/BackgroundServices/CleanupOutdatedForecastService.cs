using SpaceshipWeather.Models;
using SpaceshipWeather.Services;

namespace SpaceshipWeather.BackgroundServices;

public class CleanupOutdatedForecastService : BackgroundService
{
    private readonly ILogger<CleanupOutdatedForecastService> _logger;
    private readonly ForecastRepository _forecastRepository;

    public CleanupOutdatedForecastService(ILogger<CleanupOutdatedForecastService> logger, ForecastRepository forecastRepository)
    {
        _logger = logger;
        _forecastRepository = forecastRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _forecastRepository.DeleteOutdatedForecasts();
            _logger.LogInformation("Outdated WeatherForecast and WeatherSnapshots were removed from database!");
            await Task.Delay(ApplicationSettings.DatabaseCleanupInterval, stoppingToken);
        }
    }
}
