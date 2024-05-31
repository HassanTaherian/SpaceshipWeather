using Microsoft.AspNetCore.Mvc;
using SpaceshipWeather.Models.Entities;
using SpaceshipWeather.Services;

namespace SpaceshipWeather.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ForecastService _forecastService;

    public WeatherForecastController(ForecastService forecastService)
    {
        _forecastService = forecastService;
    }

    [HttpGet("forecasts")]
    public async Task<IActionResult> GetWeatherForecasts()
    {
        WeatherForecast? forecastDto = await _forecastService.GetForecast();

        if (forecastDto is null)
        {
            return NotFound("Haven't found any forecast!");
        }

        return Ok(forecastDto);
    }
}
