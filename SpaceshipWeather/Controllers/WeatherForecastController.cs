using Microsoft.AspNetCore.Mvc;

namespace SpaceshipWeather.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet(Name = "test")]
    public IEnumerable<string> GetTest()
    {
        return Enumerable.Empty<string>();
    }
}
