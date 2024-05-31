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

    [HttpGet("forcasts")]
    public async Task<IActionResult> GetWeatherForcasts()
    {
        WeatherForcast? forcastDto = await _forecastService.GetForcast();

        if (forcastDto is null)
        {
            return NotFound("Haven't found any forecast!");
        }

        return Ok(forcastDto);
    }
}
