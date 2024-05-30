using Dapper;
using Microsoft.Data.SqlClient;
using SpaceshipWeather.Models;
using SpaceshipWeather.Models.Entities;
using System;
using System.Data;

namespace SpaceshipWeather.Services;

public class ForecastRepository
{
    public async Task<bool> Insert(WeatherForcast weatherForecast)
    {
        using IDbConnection connection = new SqlConnection(ApplicationSettings.ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            long forecastId = await InsertWeatherForcast(weatherForecast, connection, transaction);

            await InsertSnapshots(weatherForecast.Snapshots, connection, transaction, forecastId);

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }

    private static async Task<long> InsertWeatherForcast(WeatherForcast weatherForecast, IDbConnection connection, IDbTransaction transaction)
    {
        const string insertForecastCommand = @"
                INSERT INTO WeatherForecast (Timezone, TimezoneAbbreviation, Elevation, MetricsTime, MetricsTemperature,
                                              MetricsRelativeHumidity, MetricsWindSpeed)
                VALUES (@Timezone, @TimezoneAbbreviation, @Elevation, @MetricsTime, @MetricsTemperature,
                        @MetricsRelativeHumidity, @MetricsWindSpeed);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";


        return await connection.ExecuteScalarAsync<int>(insertForecastCommand, new
        {
            weatherForecast.Timezone,
            weatherForecast.TimezoneAbbreviation,
            weatherForecast.Elevation,
            MetricsTime = weatherForecast.Metrics.Time,
            MetricsTemperature = weatherForecast.Metrics.Temperature,
            MetricsRelativeHumidity = weatherForecast.Metrics.RelativeHumidity,
            MetricsWindSpeed = weatherForecast.Metrics.WindSpeed
        }, transaction);
    }

    private static async Task InsertSnapshots(IEnumerable<WeatherSanpshot> snapshots, IDbConnection connection,
                                              IDbTransaction transaction, long forecastId)
    {
        DynamicParameters parameters = new();
        parameters.Add("@Snapshots", ToDataTable(snapshots, forecastId),
                        DbType.Object, ParameterDirection.Input);
        await connection.ExecuteAsync("sp_InsertSnapshotBatch", parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
    }

    private static DataTable ToDataTable(IEnumerable<WeatherSanpshot> sanpshots, long forecastId)
    {
        DataTable dataTable = new();
        dataTable.Columns.Add(nameof(WeatherSanpshot.TimeStamp), typeof(DateTime));
        dataTable.Columns.Add(nameof(WeatherSanpshot.RelativeHumidity), typeof(int));
        dataTable.Columns.Add(nameof(WeatherSanpshot.Temperature), typeof(decimal));
        dataTable.Columns.Add(nameof(WeatherSanpshot.WindSpeed), typeof(decimal));
        dataTable.Columns.Add(nameof(WeatherSanpshot.WeatherForecastId), typeof(long));

        foreach (WeatherSanpshot snapshot in sanpshots)
        {
            dataTable.Rows.Add(snapshot.TimeStamp, snapshot.RelativeHumidity, snapshot.Temperature,
                               snapshot.WindSpeed, forecastId);
        }

        return dataTable;
    }
}
