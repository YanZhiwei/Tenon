using System.Net;
using System.Security.Claims;
using CleanArchitecture.Identity.Application.Dtos;
using CleanArchitecture.Identity.Repository.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Tenon.AspNetCore.Abstractions.Application;
using Tenon.Models.Dtos;
using Tenon.Repository.EfCore.Transaction;

namespace CleanArchitecture.Identity.Application.Services.Impl;

public sealed class UserService : ServiceBase, IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public UserService(ILogger<UserService> logger, UserManager<User> userManager, RoleManager<Role> roleManager,
        IPasswordHasher<User> passwordHasher, IUnitOfWork unitOfWork)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(_userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(_roleManager));
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
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
        try
        {
            _unitOfWork.BeginTransaction();
            user = new User
            {
                Account = input.Account,
                UserName = input.Name,
                Email = input.Email,
                Id = 5,
                AccessFailedCount = 5,
                PhoneNumber = input.Phone,
                EmailConfirmed = false,
                Birthday = input.Birthday,
                Avatar = input.Avatar,
                DeptId = input.DeptId,
                Status = 3,
                Sex = input.Sex
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, input.Password);
            user.CreateTime = DateTime.UtcNow;
            user.CreateBy = 2;
            user.SecurityStamp = Guid.NewGuid().ToString();
            var createdResult = await _userManager.CreateAsync(user);
            if (!createdResult.Succeeded)
            {
                var errorDetails = string.Join(", ",
                    createdResult.Errors.Select(x => "Code " + x.Code + " Description" + x.Description));
                return Problem(HttpStatusCode.BadRequest, errorDetails);
            }

            var userClaim = new Claim("email", "yanzhiwei@hotmail.com");
            var result = await _userManager.AddClaimAsync(user, userClaim);
            await _unitOfWork.CommitAsync();
            return user.Id;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Problem(HttpStatusCode.BadRequest, "Create user failed");
        }
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