namespace CleanArchitecture.Identity.Application.Dtos;

public class UserLoginResultDto
{
    public long Id { get; init; }

    public string Account { get; init; }

    public string Name { get; init; }

    public string Email { get; set; }

    public string RoleIds { get; init; }

    public int Status { get; init; }
}