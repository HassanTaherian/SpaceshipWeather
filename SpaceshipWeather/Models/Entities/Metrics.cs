namespace SpaceshipWeather.Models.Entities;

public class Metrics
{
    public required string Time { get; set; }

    public required string Temperature { get; set; }

    public required string RelativeHumidity { get; set; }

    public required string WindSpeed { get; set; }
}