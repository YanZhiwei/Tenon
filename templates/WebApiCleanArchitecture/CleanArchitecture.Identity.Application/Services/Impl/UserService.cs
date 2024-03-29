using System.Net;
using CleanArchitecture.Identity.Application.Dtos;
using Microsoft.AspNetCore.Identity;
using Tenon.AspNetCore.Abstractions.Application;

namespace CleanArchitecture.Identity.Application.Services.Impl;

public sealed class UserService : ServiceBase, IUserService
{
    private readonly RoleManager<IdentityRole<long>> _roleManager;
    private readonly UserManager<IdentityUser<long>> _userManager;

    public UserService(UserManager<IdentityUser<long>> userManager, RoleManager<IdentityRole<long>> roleManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleManager = roleManager;
    }

    public async Task<ServiceResult<UserLoginResultDto>> LoginAsync(UserLoginDto input)
    {
        var existingUser = await _userManager.FindByIdAsync(input.Account);
        if (existingUser != null) return Problem(HttpStatusCode.BadRequest, "Incorrect username or password");

        return new UserLoginResultDto
        {
            Account = existingUser.NormalizedUserName,
            Email = existingUser.Email,
            Id = existingUser.Id,
            Name = existingUser.UserName
        };
    }
}