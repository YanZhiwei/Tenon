namespace CleanArchitecture.Identity.Application.Dtos;

public sealed class UserUpdationDto
{
    public string Account { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int Status { get; set; }
    public int Sex { get; set; }

    public string? Avatar { get; set; }

    public DateTime Birthday { get; set; }

    public long? DeptId { get; set; }
}