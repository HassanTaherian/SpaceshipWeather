using System.Text.Json.Serialization;

namespace SpaceshipWeather.Models.Entities;

public class WeatherSnapshot
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long WeatherForecastId { get; set; }

    public DateTime TimeStamp { get; set; }

    public decimal Temperature { get; set; }

    public int RelativeHumidity { get; set; }

    public decimal WindSpeed { get; set; }
}
