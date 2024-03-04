using System.Text;

namespace Tenon.AspNetCore.Configuration;

public class JwtOptions
{
    public Encoding Encoding => Encoding.UTF8;


    public bool ValidateIssuer { get; set; } = default;


    public string ValidIssuer { get; set; } = string.Empty;


    public bool ValidateIssuerSigningKey { get; set; } = default;


    public string SymmetricSecurityKey { get; set; } = string.Empty;


    public string IssuerSigningKey { get; set; } = string.Empty;


    public bool ValidateAudience { get; set; } = default!;


    public string ValidAudience { get; set; } = string.Empty;


    public string RefreshTokenAudience { get; set; } = string.Empty;


    public bool ValidateLifetime { get; set; } = default!;


    public bool RequireExpirationTime { get; set; }


    public int ClockSkew { get; set; } = default;


    public int Expire { get; set; } = default;


    public int RefreshTokenExpire { get; set; } = default;
}