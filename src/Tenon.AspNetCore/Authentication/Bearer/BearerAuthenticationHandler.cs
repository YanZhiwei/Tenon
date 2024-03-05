using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Tenon.AspNetCore.Authentication.Bearer;

public abstract class BearerAuthenticationHandler : AuthenticationHandler<BearerSchemeOptions>
{
    protected BearerAuthenticationHandler(IOptionsMonitor<BearerSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected BearerAuthenticationHandler(IOptionsMonitor<BearerSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        AuthenticateResult authResult;
        var authHeader = Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith(BearerDefaults.AuthenticationScheme,
                StringComparison.CurrentCultureIgnoreCase))
        {
            var startIndex = BearerDefaults.AuthenticationScheme.Length + 1;
            var token = authHeader[startIndex..].Trim();
            if (string.IsNullOrWhiteSpace(token))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                authResult = AuthenticateResult.Fail("Invalid authorization token,token is empty");
                return await Task.FromResult(authResult);
            }

            var parameters = GenerateTokenValidationParameters();
            ClaimsPrincipal principal;
            try
            {
                principal = ValidateToken(token, parameters, out _);
            }
            catch (SecurityTokenExpiredException)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                authResult = AuthenticateResult.Fail("Invalid authorization token,'exp' claim is < DateTime.UtcNow.");
                return await Task.FromResult(authResult);
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                authResult = AuthenticateResult.Fail("Invalid authorization token");
                return await Task.FromResult(authResult);
            }

            var claims = await GetValidatedInfoAsync(principal);
            if (claims?.Any() ?? false)
            {
                var identity = new ClaimsIdentity(claims, BearerDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(identity);
                authResult =
                    AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal,
                        BearerDefaults.AuthenticationScheme));
                if (Options.OnTokenValidated != null)
                {
                    var validatedContext = new BearerTokenValidatedContext(Context, Scheme, Options)
                    {
                        Principal = claimsPrincipal
                    };
                    await Options.OnTokenValidated.Invoke(validatedContext);
                }

                return await Task.FromResult(authResult);
            }

            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            authResult = AuthenticateResult.Fail("Invalid authorization token, claims is null");
            return await Task.FromResult(authResult);
        }

        Response.StatusCode = (int)HttpStatusCode.Forbidden;
        authResult = AuthenticateResult.Fail("Invalid authorization header");
        return await Task.FromResult(authResult);
    }

    protected abstract ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters,
        out SecurityToken validatedToken);

    protected abstract TokenValidationParameters GenerateTokenValidationParameters();

    protected abstract Task<Claim[]> GetValidatedInfoAsync(ClaimsPrincipal principal);
}