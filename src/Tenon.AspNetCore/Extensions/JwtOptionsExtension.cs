using Microsoft.IdentityModel.Tokens;
using Tenon.AspNetCore.Configuration;

namespace Tenon.AspNetCore.Extensions
{
    public static class JwtOptionsExtension
    {
        public static TokenValidationParameters GenerateTokenValidationParameters(this JwtOptions tokenConfig)
        {
            if (tokenConfig == null) throw new ArgumentNullException(nameof(tokenConfig));
            return new TokenValidationParameters()
            {
                ValidateIssuer = tokenConfig.ValidateIssuer,
                ValidIssuer = tokenConfig.ValidIssuer,
                ValidateIssuerSigningKey = tokenConfig.ValidateIssuerSigningKey,
                IssuerSigningKey =
                    new SymmetricSecurityKey(tokenConfig.Encoding.GetBytes(tokenConfig.SymmetricSecurityKey)),
                ValidateAudience = tokenConfig.ValidateAudience,
                ValidAudience = tokenConfig.ValidAudience,
                ValidateLifetime = tokenConfig.ValidateLifetime,
                RequireExpirationTime = tokenConfig.RequireExpirationTime,
                ClockSkew = TimeSpan.FromSeconds(tokenConfig.ClockSkew)
            };
        }
    }
}
