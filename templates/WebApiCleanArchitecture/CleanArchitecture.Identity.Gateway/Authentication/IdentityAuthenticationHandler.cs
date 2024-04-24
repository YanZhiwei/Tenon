using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using CleanArchitecture.Identity.Repository.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Tenon.AspNetCore.Authentication.Bearer;
using Tenon.AspNetCore.Authentication.Bearer.Jwt;
using Tenon.AspNetCore.Configuration;

namespace CleanArchitecture.Identity.Gateway.Authentication;

/// <summary>
///     确定用户身份的一个过程
/// </summary>
public class IdentityAuthenticationHandler : JwtBearerAuthenticationHandler
{
    private readonly UserManager<User> _userManager;

    public IdentityAuthenticationHandler(IOptionsMonitor<BearerSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock, IOptionsMonitor<JwtOptions> jwtOptions,
        UserManager<User> userManager) : base(options, logger, encoder,
        clock, jwtOptions)
    {
        _userManager = userManager;
    }

    public IdentityAuthenticationHandler(IOptionsMonitor<BearerSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, IOptionsMonitor<JwtOptions> jwtOptions, UserManager<User> userManager) : base(options,
        logger, encoder, jwtOptions)
    {
        _userManager = userManager;
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

        var userValidatedInfo = await _userManager.GetUserAsync(principal);

        if (userValidatedInfo is null)
            return [];
        var userClaims = await _userManager.GetClaimsAsync(userValidatedInfo);
        if (!(userClaims?.Any() ?? false))
            return [];
        claims.Add(new Claim(type: ClaimTypes.Name, value: userValidatedInfo.UserName));
        claims.Add(new Claim(type: JwtRegisteredClaimNames.UniqueName, value: userValidatedInfo.Account));
        claims.AddRange(userClaims);
        return claims.ToArray();
    }
}