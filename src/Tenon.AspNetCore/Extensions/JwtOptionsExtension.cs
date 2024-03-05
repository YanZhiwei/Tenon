using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Tenon.AspNetCore.Configuration;
using Tenon.AspNetCore.Models;

namespace Tenon.AspNetCore.Extensions;

public static class JwtOptionsExtension
{
    public static TokenValidationParameters GenerateTokenValidationParameters(this JwtOptions jwtOptions)
    {
        if (jwtOptions == null) throw new ArgumentNullException(nameof(jwtOptions));
        return new TokenValidationParameters
        {
            ValidateIssuer = jwtOptions.ValidateIssuer,
            ValidIssuer = jwtOptions.ValidIssuer,
            ValidateIssuerSigningKey = jwtOptions.ValidateIssuerSigningKey,
            IssuerSigningKey =
                new SymmetricSecurityKey(jwtOptions.Encoding.GetBytes(jwtOptions.SymmetricSecurityKey)),
            ValidateAudience = jwtOptions.ValidateAudience,
            ValidAudience = jwtOptions.ValidAudience,
            ValidateLifetime = jwtOptions.ValidateLifetime,
            RequireExpirationTime = jwtOptions.RequireExpirationTime,
            ClockSkew = TimeSpan.FromSeconds(jwtOptions.ClockSkew)
        };
    }

    public static JwtBearerToken CreateAccessToken(this JwtOptions jwtOptions, Claim[] claims)
    {
        if (jwtOptions == null) throw new ArgumentNullException(nameof(jwtOptions));
        if (!(claims?.Any() ?? false))
            throw new ArgumentNullException(nameof(claims));
        return WriteToken(jwtOptions, claims);
    }

    public static JwtBearerToken CreateRefreshToken(this JwtOptions jwtOptions, Claim[] claims)
    {
        if (jwtOptions == null) throw new ArgumentNullException(nameof(jwtOptions));
        if (!(claims?.Any() ?? false))
            throw new ArgumentNullException(nameof(claims));
        return WriteToken(jwtOptions, claims, false);
    }

    private static JwtBearerToken WriteToken(JwtOptions jwtConfig, Claim[] claims, bool accessToken = true)
    {
        var key = new SymmetricSecurityKey(jwtConfig.Encoding.GetBytes(jwtConfig.SymmetricSecurityKey));

        var issuer = jwtConfig.ValidIssuer;
        var audience = accessToken ? jwtConfig.ValidAudience : jwtConfig.RefreshTokenAudience;
        var expiresMinutes = accessToken ? jwtConfig.Expire : jwtConfig.RefreshTokenExpire;
        var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            DateTime.UtcNow,
            expires,
            new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
        return new JwtBearerToken(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}