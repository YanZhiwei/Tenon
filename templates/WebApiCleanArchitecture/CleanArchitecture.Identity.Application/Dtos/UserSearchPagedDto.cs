using Tenon.Abstractions;

namespace CleanArchitecture.Identity.Application.Dtos;

public sealed class UserSearchPagedDto : SearchPagedDto
{
    public string? Name { get; set; }

    public string? Account { get; set; }
}