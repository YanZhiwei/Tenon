using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tenon.AspNetCore.Configuration;
using Tenon.AspNetCore.Extensions;

namespace Tenon.AspNetCore.Authentication.Bearer.Jwt;

/// <summary>
///     Jwt验证(认证)服务
/// </summary>
public abstract class JwtBearerAuthenticationHandler : BearerAuthenticationHandler
{
    protected readonly IOptionsMonitor<JwtOptions> JwtOptions;

    protected JwtBearerAuthenticationHandler(IOptionsMonitor<BearerSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock, IOptionsMonitor<JwtOptions> jwtOptions) : base(options, logger, encoder,
        clock)
    {
        JwtOptions = jwtOptions;
    }

    protected JwtBearerAuthenticationHandler(IOptionsMonitor<BearerSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, IOptionsMonitor<JwtOptions> jwtOptions) : base(options, logger, encoder)
    {
        JwtOptions = jwtOptions;
    }

    protected override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters parameters,
        out SecurityToken validatedToken)
    {
        var jwtSecurityHandler = new JwtSecurityTokenHandler();
        var principal = jwtSecurityHandler.ValidateToken(token, parameters, out validatedToken);
        return principal;
    }

    protected override TokenValidationParameters GenerateTokenValidationParameters()
    {
        return JwtOptions.CurrentValue.GenerateTokenValidationParameters();
    }
}