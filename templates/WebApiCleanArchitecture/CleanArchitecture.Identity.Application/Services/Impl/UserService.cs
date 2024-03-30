using System.Net;
using CleanArchitecture.Identity.Application.Dtos;
using CleanArchitecture.Identity.Repository.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Tenon.AspNetCore.Abstractions.Application;
using Tenon.Models.Dtos;

namespace CleanArchitecture.Identity.Application.Services.Impl;

public sealed class UserService : ServiceBase, IUserService
{
    private readonly ILogger<UserService> _logger;
    public readonly IPasswordHasher<User> _passwordHasher;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public UserService(ILogger<UserService> logger, UserManager<User> userManager, RoleManager<Role> roleManager,
        IPasswordHasher<User> passwordHasher)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(_userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(_roleManager));
        _passwordHasher = passwordHasher;
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

    public async Task<ServiceResult<long>> CreateAsync(UserCreationDto input)
    {
        var user = await _userManager.FindByNameAsync(input.Name);
        if (user != null)
            return Problem(HttpStatusCode.BadRequest, $"UserName:{input.Name} is exist");
        user = new User
        {
            Account = input.Account,
            UserName = input.Name,
            Email = input.Email,
            Id = 1,
            AccessFailedCount = 5,
            PhoneNumber = input.Phone,
            EmailConfirmed = false,
            Birthday = input.Birthday,
            Avatar = input.Avatar,
            DeptId = input.DeptId,
            Status = 1,
            Sex = input.Sex
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, input.Password);
        user.CreateTime = DateTime.UtcNow;
        user.CreateBy = 1;
        user.SecurityStamp = Guid.NewGuid().ToString();
        var createdResult = await _userManager.CreateAsync(user);
        if (!createdResult.Succeeded)
            return Problem(HttpStatusCode.BadRequest, createdResult.Errors.FirstOrDefault().Description);
        return user.Id;
    }

    public Task<ServiceResult> UpdateAsync(long id, UserUpdationDto input)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<PagedListDto<UserDto>> GetPagedAsync(UserSearchPagedDto search)
    {
        throw new NotImplementedException();
    }
}