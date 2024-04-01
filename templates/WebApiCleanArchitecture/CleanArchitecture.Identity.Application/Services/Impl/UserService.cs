using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using CleanArchitecture.Identity.Application.Dtos;
using CleanArchitecture.Identity.Repository.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Tenon.AspNetCore.Abstractions.Application;
using Tenon.Mapper.Abstractions;
using Tenon.Models.Dtos;
using Tenon.Repository.EfCore.Transaction;

namespace CleanArchitecture.Identity.Application.Services.Impl;

public sealed class UserService : ServiceBase, IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IObjectMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;

    public UserService(ILogger<UserService> logger, UserManager<User> userManager, RoleManager<Role> roleManager,
        IPasswordHasher<User> passwordHasher, IUnitOfWork unitOfWork, IObjectMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(_userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(_roleManager));
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
        var existUser = await _userManager.FindByNameAsync(input.Name);
        if (existUser != null)
            return Problem(HttpStatusCode.BadRequest, $"UserName:{input.Name} is exist");

        try
        {
            _unitOfWork.BeginTransaction();
            var user = _mapper.Map<User>(input);
            user.PasswordHash = _passwordHasher.HashPassword(user, input.Password);
            user.CreateTime = DateTime.UtcNow;
            user.CreateBy = 2;
            user.SecurityStamp = Guid.NewGuid().ToString();
            var createdResult = await _userManager.CreateAsync(user);
            if (!createdResult.Succeeded)
                return Problem(HttpStatusCode.BadRequest, $"Create user:{input.Name} failed");

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Email, input.Email),
                new(JwtRegisteredClaimNames.Birthdate, input.Birthday.ToString()),
                new(JwtRegisteredClaimNames.Gender, input.Sex.ToString()),
                new(IdentityRegisteredClaimNames.DeptId, input.DeptId.ToString())
            };
            var addClaimsResult = await _userManager.AddClaimsAsync(user, claims);
            if (!addClaimsResult.Succeeded)
            {
                await _unitOfWork.RollbackAsync();
                return Problem(HttpStatusCode.BadRequest, $"Create user:{input.Name} failed");
            }

            await _unitOfWork.CommitAsync();
            return user.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Create user:{input.Name} failed");
            await _unitOfWork.RollbackAsync();
            return Problem(HttpStatusCode.BadRequest, $"Create user:{input.Name} failed");
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