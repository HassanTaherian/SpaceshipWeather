
using SpaceshipWeather.Models.Entities;
using SpaceshipWeather.Services;
using System.Threading.Channels;

namespace SpaceshipWeather.BackgroundServices;

public class InsertSnapshotBatchtoDatabaseService : BackgroundService
{
    public readonly ChannelReader<WeatherForecast> _channelReader;
    private readonly ForecastRepository _forecastRepository;

    public InsertSnapshotBatchtoDatabaseService(ChannelReader<WeatherForecast> channelReader,
                                                ForecastRepository forecastRepository)
    {
        _channelReader = channelReader;
        _forecastRepository = forecastRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            WeatherForecast weatherForecast = await _channelReader.ReadAsync(stoppingToken);

            await _forecastRepository.Insert(weatherForecast);
        }
    }
}
