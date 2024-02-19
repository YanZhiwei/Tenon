using Microsoft.AspNetCore.Mvc;

namespace ConsulRefitSample.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IWeatherForecastApi _forecastApi;

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastApi forecastApi)
    {
        _logger = logger;
        _forecastApi = forecastApi;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return _forecastApi.GetForecastAsync(DateTime.Now).GetAwaiter().GetResult();
        ;
    }
}