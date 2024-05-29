namespace SpaceshipWeather.Models.Entities;

public class WeatherSanpshot
{
    public DateTime TimeStamp { get; set; }

    public decimal Temperature { get; set; }

    public int RelativeHumidity { get; set; }

    public decimal WindSpeed { get; set; }
}
