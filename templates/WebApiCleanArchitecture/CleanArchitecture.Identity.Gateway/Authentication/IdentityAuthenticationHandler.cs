using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Tenon.AspNetCore.Authentication.Bearer;
using Tenon.AspNetCore.Authentication.Bearer.Jwt;
using Tenon.AspNetCore.Configuration;

namespace CleanArchitecture.Identity.Gateway.Authentication;

public class IdentityAuthenticationHandler : JwtBearerAuthenticationHandler
{
    public IdentityAuthenticationHandler(IOptionsMonitor<BearerSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock, IOptionsMonitor<JwtOptions> jwtOptions) : base(options, logger, encoder,
        clock, jwtOptions)
    {
    }

    public IdentityAuthenticationHandler(IOptionsMonitor<BearerSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, IOptionsMonitor<JwtOptions> jwtOptions) : base(options,
        logger, encoder, jwtOptions)
    {
    }

    protected override async Task<Claim[]> GetValidatedInfoAsync(ClaimsPrincipal principal)
    {
        var claims = principal.Claims.ToList();
        if (!(claims?.Any() ?? false)) return [];
        var subClaim = claims.FirstOrDefault(x =>
            x.Type == ClaimTypes.NameIdentifier);
        if (subClaim is null)
            return [];

        var subClaimParse = long.TryParse(subClaim.Value, out var userId);
        if (subClaimParse == false)
            return [];

        var jtiClaim = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti);
        if (jtiClaim is null)
            return [];

        var jtiClaimParse = jtiClaim.Value;

        ////var userValidatedInfo = await _userManager.GetUserAsync(principal);

        ////if (userValidatedInfo is null)
        ////    return [];
        ////var userClaims = await _userManager.GetClaimsAsync(userValidatedInfo);
        ////if (!(userClaims?.Any() ?? false))
        ////    return [];
        ////claims.Add(new Claim(ClaimTypes.Name, userValidatedInfo.UserName));
        //claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, userValidatedInfo.Account));
        //claims.AddRange(userClaims);
        return claims.ToArray();
    }
}