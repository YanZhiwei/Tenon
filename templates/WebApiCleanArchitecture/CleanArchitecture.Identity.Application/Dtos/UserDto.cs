namespace CleanArchitecture.Identity.Application.Dtos;

public sealed class UserDto
{
    public string Account { get; set; } = string.Empty;


    public string Avatar { get; set; } = string.Empty;


    public DateTime? Birthday { get; set; }


    public long? DeptId { get; set; }


    public string DeptName { get; set; } = string.Empty;


    public string? Email { get; set; } = string.Empty;


    public string Name { get; set; } = string.Empty;


    public string Phone { get; set; } = string.Empty;


    public int? Sex { get; set; }


    public int Status { get; set; }
}