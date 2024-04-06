namespace CleanArchitecture.Identity.Application.Dtos;

public class UserLoginResultDto(string token, DateTime expire, string refreshToken, DateTime refreshExpire)
{
    public string Token { get; set; } = token;


    public DateTime Expire { get; set; } = expire;


    public string RefreshToken { get; set; } = refreshToken;


    public DateTime RefreshExpire { get; set; } = refreshExpire;
}