using System.Text.Json.Serialization;

namespace SpaceshipWeather.Models.Entities;

public class WeatherForecast
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long WeatherForecastId { get; set; }

    public required string Timezone { get; set; }

    public required string TimezoneAbbreviation { get; set; }

    public double Elevation { get; set; }

    public required Metrics Metrics { get; set; }

    public required IEnumerable<WeatherSnapshot> Snapshots { get; set; }

}
