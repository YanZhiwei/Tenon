using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tenon.AspNetCore.Configuration;
using Tenon.AspNetCore.Extensions;
using WebApiJwtBearerSample.Dtos;

namespace WebApiJwtBearerSample.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly JwtOptions _jwtOptions;

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptionsMonitor<JwtOptions> jwtOptions)
    {
        _logger = logger;
        _jwtOptions = jwtOptions.CurrentValue;
    }

    [Authorize]
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }


    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserTokenInfoDto>> LoginAsync([FromBody] UserLoginDto input)
    {
        var claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new Claim(JwtRegisteredClaimNames.UniqueName, input.Account),
            new Claim(JwtRegisteredClaimNames.NameId, "1"),
            new Claim(JwtRegisteredClaimNames.Name, input.Account),
            new Claim(ClaimTypes.Role, "admin")
        };
        var accessToken = _jwtOptions.CreateAccessToken(claims);
        var refreshToken = _jwtOptions.CreateRefreshToken(claims);
        var tokenInfo = new UserTokenInfoDto(accessToken.Token, accessToken.Expire, refreshToken.Token,
            refreshToken.Expire);
        return Created("/auth/session", tokenInfo);
    }
}