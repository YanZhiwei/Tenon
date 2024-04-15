using CleanArchitecture.Identity.Application.Dtos;
using Tenon.Abstractions;
using Tenon.AspNetCore.Abstractions.Application;
using Tenon.Models.Dtos;

namespace CleanArchitecture.Identity.Application.Services;

public interface IUserService : IAppService
{
    Task<ServiceResult<UserLoginResultDto>> LoginAsync(UserLoginDto input);

    Task<ServiceResult<UserTokenInfoDto>> RefreshAccessTokenAsync(UserRefreshTokenDto input);

    Task<ServiceResult<long>> CreateAsync(UserCreationDto input);

    Task<ServiceResult> UpdateAsync(long id, UserUpdationDto input);

    Task<ServiceResult> DeleteAsync(long id);

    Task<PagedResultDto<UserDto>> GetPagedAsync(UserSearchPagedDto search);


}