using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tenon.AspNetCore.Authentication.Basic;

/// <summary>
///     Basic验证(认证)服务
/// </summary>
public abstract class BasicAuthenticationHandler : AuthenticationHandler<BasicSchemeOptions>
{
    protected BasicAuthenticationHandler(IOptionsMonitor<BasicSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected BasicAuthenticationHandler(IOptionsMonitor<BasicSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        AuthenticateResult authResult;
        var authHeader = Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith(BasicDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
        {
            var startIndex = BasicDefaults.AuthenticationScheme.Length + 1;
            var token = authHeader[startIndex..].Trim();
            if (string.IsNullOrWhiteSpace(token))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                authResult = AuthenticateResult.Fail("Invalid authorization token,detail:token is empty.");
                return await Task.FromResult(authResult);
            }

            var claims = UnPackFromBase64(token);
            if (claims?.Any() ?? false)
            {
                var identity = new ClaimsIdentity(claims, BasicDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(identity);
                authResult =
                    AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, BasicDefaults.AuthenticationScheme));
                var validatedContext = new BasicTokenValidatedContext(Context, Scheme, Options)
                {
                    Principal = claimsPrincipal
                };
                if (Options.OnTokenValidated != null)
                    await Options.OnTokenValidated.Invoke(validatedContext);
                return await Task.FromResult(authResult);
            }
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            authResult = AuthenticateResult.Fail("Invalid authorization token,detail:unPack from base64 failed");
            return await Task.FromResult(authResult);
        }

        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        authResult = AuthenticateResult.Fail("Invalid authorization header");
        return await Task.FromResult(authResult);
    }

    protected abstract Claim[] UnPackFromBase64(string token);
}