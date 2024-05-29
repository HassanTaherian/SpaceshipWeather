using System.Text.Json.Serialization;

namespace SpaceshipWeather.Models.Dtos;

public class MetricsDto
{
    [JsonPropertyName("time")]
    public required string Time { get; set; }

    [JsonPropertyName("temperature_2m")]
    public required string Temperature { get; set; }

    [JsonPropertyName("relativehumidity_2m")]
    public required string RelativeHumidity { get; set; }

    [JsonPropertyName("windspeed_10m")]
    public required string WindSpeed { get; set; }
}