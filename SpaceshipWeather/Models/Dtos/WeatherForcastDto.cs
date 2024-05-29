namespace SpaceshipWeather.Models.Dtos;

using System.Text.Json.Serialization;

public class WeatherForcastDto
{
    [JsonPropertyName("timezone")]
    public required string Timezone { get; set; }

    [JsonPropertyName("timezone_abbreviation")]
    public required string TimezoneAbbreviation { get; set; }

    [JsonPropertyName("elevation")]
    public double Elevation { get; set; }

    [JsonPropertyName("hourly_units")]
    public required MetricsDto Metrics { get; set; }

    [JsonPropertyName("hourly")]
    public required SnapshotsDataDto SnapshotsData { get; set; }
}
