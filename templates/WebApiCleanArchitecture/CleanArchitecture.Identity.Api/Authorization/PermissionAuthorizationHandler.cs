using CleanArchitecture.Identity.Api.Models;
using Tenon.AspNetCore.Authorization.Bearer;

namespace CleanArchitecture.Identity.Api.Authorization
{
    /// <summary>
    /// 确认用户可以做哪些事情，即权限
    /// </summary>
    public sealed class PermissionAuthorizationHandler : BearAuthorizationHandler
    {
        protected override async Task<bool> CheckUserPermissionsAsync(HttpContext httpContext, string[]? codes)
        {
            var userContext = httpContext.RequestServices.GetService<UserContext>();
            return await Task.FromResult(userContext != null);
        }
    }
}
