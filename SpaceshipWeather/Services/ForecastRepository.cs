using Dapper;
using Microsoft.Data.SqlClient;
using SpaceshipWeather.Models;
using SpaceshipWeather.Models.Entities;
using System;
using System.Data;

namespace SpaceshipWeather.Services;

public class ForecastRepository
{
    private readonly IConfiguration _configuration;

    public ForecastRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> Insert(WeatherForecast weatherForecast)
    {
        string? connectionString = _configuration.GetConnectionString(ApplicationSettings.ConnectionStringName);
        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        try
        {
            long forecastId = await InsertWeatherForecast(weatherForecast, connection, transaction);

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

    private static async Task<long> InsertWeatherForecast(WeatherForecast weatherForecast, IDbConnection connection, IDbTransaction transaction)
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

    private static async Task InsertSnapshots(IEnumerable<WeatherSnapshot> snapshots, IDbConnection connection,
                                              IDbTransaction transaction, long forecastId)
    {
        DynamicParameters parameters = new();
        parameters.Add("@Snapshots", ToDataTable(snapshots, forecastId),
                        DbType.Object, ParameterDirection.Input);
        await connection.ExecuteAsync("sp_InsertSnapshotBatch", parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
    }

    private static DataTable ToDataTable(IEnumerable<WeatherSnapshot> snapshots, long forecastId)
    {
        DataTable dataTable = new();
        dataTable.Columns.Add(nameof(WeatherSnapshot.TimeStamp), typeof(DateTime));
        dataTable.Columns.Add(nameof(WeatherSnapshot.RelativeHumidity), typeof(int));
        dataTable.Columns.Add(nameof(WeatherSnapshot.Temperature), typeof(decimal));
        dataTable.Columns.Add(nameof(WeatherSnapshot.WindSpeed), typeof(decimal));
        dataTable.Columns.Add(nameof(WeatherSnapshot.WeatherForecastId), typeof(long));

        foreach (WeatherSnapshot snapshot in snapshots)
        {
            dataTable.Rows.Add(snapshot.TimeStamp, snapshot.RelativeHumidity, snapshot.Temperature,
                               snapshot.WindSpeed, forecastId);
        }

        return dataTable;
    }

    // Use transactions or use join
    public async Task<WeatherForecast?> FetchLastForecast()
    {
        string? connectionString = _configuration.GetConnectionString(ApplicationSettings.ConnectionStringName);
        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        const string selectMostRecentForecastQuery = @"
                SELECT [TimeStamp],
                       Temperature,
                       RelativeHumidity,
                       WindSpeed,
                       Timezone,
                       TimezoneAbbreviation,
                       Elevation,
                       MetricsTime AS Time,
                       MetricsTemperature AS Temperature,
                       MetricsRelativeHumidity AS RelativeHumidity,
                       MetricsWindSpeed AS WindSpeed
                FROM WeatherSnapshot
                INNER JOIN (SELECT WeatherForecastId,
				                    Timezone,
				                    TimezoneAbbreviation,
				                    Elevation,
				                    MetricsTime,
				                    MetricsTemperature,
				                    MetricsRelativeHumidity,
				                    MetricsWindSpeed
			                FROM WeatherForecast
			                WHERE CreatedAt = (
				                SELECT MAX(CreatedAt)
				                FROM WeatherForecast
			                )) AS WeatherForecast ON WeatherForecast.WeatherForecastId = WeatherSnapshot.WeatherForecastId;
        ";

        WeatherForecast? result = null;

        await connection.QueryAsync<WeatherSnapshot, WeatherForecast, Metrics, WeatherForecast>(selectMostRecentForecastQuery,
                                    (snapshot, forecast, metrics) =>
                                    {
                                        result ??= forecast;
                                        result.Snapshots ??= [];

                                        result.Snapshots.Add(snapshot);
                                        result.Metrics ??= metrics;
                                        return forecast;
                                    },
                                    splitOn: "TimeZone,Time");
        return result;
    }


    public async Task<bool> DeleteOutdatedForecasts()
    {
        string? connectionString = _configuration.GetConnectionString(ApplicationSettings.ConnectionStringName);
        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        try
        {
            await DeleteOutdatedWeatherSnapshotRecords(connection, transaction);

            await DeleteOutDatedWeatherForecastRecords(connection, transaction);

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }

    private static async Task DeleteOutdatedWeatherSnapshotRecords(IDbConnection connection, IDbTransaction transaction)
    {
        const string deleteOutdatedSnapshotsCommand = @"
                DELETE FROM WeatherSnapshot
                WHERE WeatherForecastId NOT IN (
                    SELECT WeatherForecastId
                    FROM WeatherForecast
                    WHERE CreatedAt = (
                        SELECT Max(CreatedAt)
                        FROM WeatherForecast
                    )
                );
            ";

        await connection.ExecuteAsync(deleteOutdatedSnapshotsCommand, transaction: transaction);
    }

    private static async Task DeleteOutDatedWeatherForecastRecords(IDbConnection connection, IDbTransaction transaction)
    {
        const string deleteOutdatedForecastsCommand = @"
                DELETE FROM WeatherForecast
                WHERE CreatedAt <> (
                    SELECT Max(CreatedAt)
                    FROM WeatherForecast
                );
            ";

        await connection.ExecuteAsync(deleteOutdatedForecastsCommand, transaction: transaction);
    }

}
