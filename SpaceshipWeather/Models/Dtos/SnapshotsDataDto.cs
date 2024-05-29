using System.Text.Json.Serialization;

namespace SpaceshipWeather.Models.Dtos;

public class SnapshotsDataDto
{
    [JsonPropertyName("time")]
    public required List<DateTime> TimeStamps { get; set; }

    [JsonPropertyName("temperature_2m")]
    public required List<decimal> Temperatures { get; set; }

    [JsonPropertyName("relativehumidity_2m")]
    public required List<int> RelativeHumidities { get; set; }

    [JsonPropertyName("windspeed_10m")]
    public required List<decimal> WindSpeeds { get; set; }
}