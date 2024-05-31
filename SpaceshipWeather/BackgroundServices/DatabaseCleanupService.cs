using SpaceshipWeather.Models;
using SpaceshipWeather.Services;

namespace SpaceshipWeather.BackgroundServices;

public class DatabaseCleanupService : BackgroundService
{
    private readonly ForecastRepository _forecastRepository;

    public DatabaseCleanupService(ForecastRepository forecastRepository)
    {
        _forecastRepository = forecastRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Console.Out.WriteLineAsync("Clean up started");
            await _forecastRepository.DeleteOutdatedForecasts();
            await Task.Delay(ApplicationSettings.DatabaseCleanupInterval, stoppingToken);
        }
    }
}
