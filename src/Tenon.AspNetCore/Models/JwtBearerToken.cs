namespace Tenon.AspNetCore.Models;

public class JwtBearerToken(string token, DateTime expire)
{
    public string Token { get; set; } = token;
    public DateTime Expire { get; set; } = expire;
}