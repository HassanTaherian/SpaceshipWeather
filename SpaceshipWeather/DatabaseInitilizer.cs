using Dapper;
using Microsoft.Data.SqlClient;
using SpaceshipWeather.Models;

namespace SpaceshipWeather;

public class DatabaseInitilizer
{
    private readonly ILogger<DatabaseInitilizer> _logger;

    public DatabaseInitilizer(ILogger<DatabaseInitilizer> logger)
    {
        _logger = logger;
    }

    public async Task Setup()
    {
        using SqlConnection connection = new(ApplicationSettings.ConnectionString);

        connection.Open();

        await CreateWeatherForecastTable(connection);
        await CreateWeatherSnapshotTable(connection);
        await CreateSnapshotBatchTable(connection);
        await CreateInsertSnapshotBatchStoredProcedure(connection);
    }

    private async Task CreateWeatherForecastTable(SqlConnection connection)
    {
        const string createWeatherForecastTableCommand = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WeatherForecast')
                BEGIN
                CREATE TABLE WeatherForecast (
                    WeatherForecastId BIGINT NOT NULL IDENTITY(1,1),
                    Timezone VARCHAR(50) NOT NULL,
                    TimezoneAbbreviation VARCHAR(10) NOT NULL,
                    Elevation FLOAT NOT NULL,
                    MetricsTime VARCHAR(25) NOT NULL,
                    MetricsTemperature VARCHAR(25) NOT NULL,
                    MetricsRelativeHumidity VARCHAR(25) NOT NULL,
                    MetricsWindSpeed VARCHAR(25) NOT NULL,
                    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
                    PRIMARY KEY (WeatherForecastId)
                );
                END
        ";

        await connection.ExecuteAsync(createWeatherForecastTableCommand);
        _logger.LogInformation("WeatherForecastTable created!");
    }   
    
    private async Task CreateWeatherSnapshotTable(SqlConnection connection)
    {
        const string createWeatherSnapshotTableCommand = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WeatherSnapshot')
                BEGIN
                CREATE TABLE WeatherSnapshot (
                    TimeStamp DATETIME2 NOT NULL,
                    Temperature DECIMAL(10,2) NOT NULL,
                    RelativeHumidity INT NOT NULL,
                    WindSpeed DECIMAL(10,2) NOT NULL,
                    WeatherForecastId BIGINT NOT NULL,
                    CONSTRAINT WeatherForecast_WeatherForecastId_FK 
                    FOREIGN KEY(WeatherForecastId) REFERENCES WeatherForecast(WeatherForecastId)
                );
                END
        ";

        await connection.ExecuteAsync(createWeatherSnapshotTableCommand);
        _logger.LogInformation("WeatherSnapshotTable created!");
    }


    private async Task CreateSnapshotBatchTable(SqlConnection connection)
    {
        const string createSnapshotBatchTableCommand = @"
                IF NOT EXISTS (SELECT * FROM sys.types WHERE name = 'WeatherSnapshotBatchTable')
                BEGIN
                    CREATE TYPE WeatherSnapshotBatchTable AS TABLE
                    (
                        TimeStamp DATETIME2 NOT NULL,
                        Temperature DECIMAL(10,2) NOT NULL,
                        RelativeHumidity INT NOT NULL,
                        WindSpeed DECIMAL(10,2) NOT NULL,
                        WeatherForecastId BIGINT NOT NULL
                    );
                END
        ";

        await connection.ExecuteAsync(createSnapshotBatchTableCommand);
        _logger.LogInformation("SnapshotBatchTable type created!");
    }

    public async Task CreateInsertSnapshotBatchStoredProcedure(SqlConnection connection)
    {
        const string createSnapshotBatchTableCommand = @"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'sp_InsertSnapshotBatch' AND type = 'P')
                BEGIN
                    EXEC('
                    CREATE PROCEDURE sp_InsertSnapshotBatch 
                        @Snapshots dbo.WeatherSnapshotBatchTable READONLY
                    AS
                    BEGIN
                        INSERT INTO WeatherSnapshot ([TimeStamp], Temperature, RelativeHumidity, WindSpeed, WeatherForecastId)
                        SELECT [TimeStamp], Temperature, RelativeHumidity, WindSpeed, WeatherForecastId 
                        FROM @Snapshots;
                    END')
                END
        ";

        await connection.ExecuteAsync(createSnapshotBatchTableCommand);
        _logger.LogInformation("sp_InsertSnapshotBatch stored procedure created!");
    }
}
