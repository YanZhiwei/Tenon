using CleanArchitecture.Identity.Application.Dtos;
using Tenon.AspNetCore.Abstractions.Application;

namespace CleanArchitecture.Identity.Application.Services;

public interface IUserService
{
    Task<ServiceResult<UserLoginResultDto>> LoginAsync(UserLoginDto input);
}