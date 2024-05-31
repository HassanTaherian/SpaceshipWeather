namespace SpaceshipWeather.Models;

public static class ApplicationSettings
{
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

    public const string BaseAddress = "https://api.open-meteo.com/v1/";

    public const string DatabaseName = "SpaceshipWeather";
    public const string ConnectionStringName = "SpaceshipWeather";

    public static readonly TimeSpan DatabaseCleanupInterval = TimeSpan.FromHours(1);

}
