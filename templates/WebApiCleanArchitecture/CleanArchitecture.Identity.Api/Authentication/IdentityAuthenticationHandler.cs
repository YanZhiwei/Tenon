using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Tenon.AspNetCore.Authentication.Bearer;
using Tenon.AspNetCore.Authentication.Bearer.Jwt;
using Tenon.AspNetCore.Configuration;

namespace CleanArchitecture.Identity.Api.Authentication
{
    public class IdentityAuthenticationHandler : JwtBearerAuthenticationHandler
    {
        public IdentityAuthenticationHandler(IOptionsMonitor<BearerSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IOptionsMonitor<JwtOptions> jwtOptions) : base(options, logger, encoder, clock, jwtOptions)
        {
        }

        public IdentityAuthenticationHandler(IOptionsMonitor<BearerSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IOptionsMonitor<JwtOptions> jwtOptions) : base(options, logger, encoder, jwtOptions)
        {
        }

        protected override async Task<Claim[]> GetValidatedInfoAsync(ClaimsPrincipal principal)
        {
            var claims = principal.Claims;

            var idClaim = claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (idClaim is null)
                return Array.Empty<Claim>();

            var parseResult = long.TryParse(idClaim.Value, out var userId);
            if (parseResult == false)
                throw new InvalidCastException(nameof(idClaim.Value));

            var validationVersion = Guid.NewGuid().ToString("N");


            var jtiClaim = claims.FirstOrDefault(x => x.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti);
            if (jtiClaim is null)
                return Array.Empty<Claim>();

            //if (validationVersion != jtiClaim.Value)
            //    return Array.Empty<Claim>();

            return claims.ToArray();
        }
    }
}
