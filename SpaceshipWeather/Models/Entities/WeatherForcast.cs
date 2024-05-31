﻿namespace SpaceshipWeather.Models.Entities;

public class WeatherForcast
{
    public long WeatherForecastId { get; set; }

    public required string Timezone { get; set; }

    public required string TimezoneAbbreviation { get; set; }

    public double Elevation { get; set; }

    public required Metrics Metrics { get; set; }

    public required IEnumerable<WeatherSanpshot> Snapshots { get; set; }

}
