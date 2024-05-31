
using SpaceshipWeather.Models.Entities;
using SpaceshipWeather.Services;
using System.Threading.Channels;

namespace SpaceshipWeather.BackgroundServices;

public class InsertSnapshotBatchtoDatabaseService : BackgroundService
{
    public readonly ChannelReader<WeatherForecast> _channelReader;
    private readonly ForecastRepository _forecastRepository;
    private readonly ILogger<InsertSnapshotBatchtoDatabaseService> _logger;

    public InsertSnapshotBatchtoDatabaseService(ChannelReader<WeatherForecast> channelReader,
                                                ForecastRepository forecastRepository,
                                                ILogger<InsertSnapshotBatchtoDatabaseService> logger)
    {
        _channelReader = channelReader;
        _forecastRepository = forecastRepository;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            WeatherForecast weatherForecast = await _channelReader.ReadAsync(stoppingToken);

            bool isSuccessful = await _forecastRepository.Insert(weatherForecast);

            if (isSuccessful)
            {
                _logger.LogInformation("Forecast was added to database!");
            }
            else
            {
                _logger.LogError("Inserting forecasts transaction failed!");
            }
        }
    }
}
