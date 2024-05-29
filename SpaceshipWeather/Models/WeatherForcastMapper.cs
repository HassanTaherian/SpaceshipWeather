﻿using SpaceshipWeather.Models.Dtos;
using SpaceshipWeather.Models.Entities;

namespace SpaceshipWeather.Models;

public class WeatherForcastMapper
{
    public WeatherForcast MapWeatherForcastDtoToWeatherForcast(WeatherForcastDto dto)
    {
        List<WeatherSanpshot> snapshots = [];

        for (int i = 0; i < dto.SnapshotsData.RelativeHumidities.Count; i++)
        {
            snapshots.Add(new WeatherSanpshot()
            {
                TimeStamp = dto.SnapshotsData.TimeStamps[i],
                RelativeHumidity = dto.SnapshotsData.RelativeHumidities[i],
                Temperature = dto.SnapshotsData.Temperatures[i],
                WindSpeed = dto.SnapshotsData.WindSpeeds[i]
            });
        }

        return new WeatherForcast()
        {
            Metrics = new Metrics()
            {
                Time = dto.Metrics.Time,
                RelativeHumidity = dto.Metrics.RelativeHumidity,
                Temperature = dto.Metrics.Temperature,
                WindSpeed = dto.Metrics.WindSpeed,
            },
            Elevation = dto.Elevation,
            Timezone = dto.Timezone,
            TimezoneAbbreviation = dto.TimezoneAbbreviation,
            Snapshots = snapshots
        };
    }
}
