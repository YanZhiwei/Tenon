namespace CleanArchitecture.Identity.Application.Dtos;

public class UserTokenInfoDto
{
    public string Token { get; set; }


    public DateTime Expire { get; set; }

    public string RefreshToken { get; set; }


    public DateTime RefreshExpire { get; set; }
}