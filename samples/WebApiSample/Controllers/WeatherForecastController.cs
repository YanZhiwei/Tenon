using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Tenon.AspNetCore.Controllers;

namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ApiControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public ActionResult<IEnumerable<WeatherForecast>> Get()
    {
        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });
        return Result(result);
    }

    [HttpPost("AddUser")]
    public ActionResult<string> AddUser([FromBody] User user)
    {
        if (user.Age == 100)
            throw new InvalidDataException("test ");
        return Result(Guid.NewGuid().ToString());
    }
}

public class User
{
    [Required(ErrorMessage = "用户Code不能为空")]
    public string Code { get; set; }

    [Required(ErrorMessage = "用户名称不能为空")]
    public string Name { get; set; }

    [Required(ErrorMessage = "用户年龄不能为空")]
    [Range(1, 100, ErrorMessage = "年龄必须介于1~100之间")]
    public int Age { get; set; }

    public string Address { get; set; }
}