using Tenon.AspNetCore.Authorization.Bearer;

namespace CleanArchitecture.Identity.Api.Authorization
{
    public sealed class PermissionAuthorizationHandler : BearAuthorizationHandler
    {
        protected override async Task<bool> CheckUserPermissionsAsync(HttpContext httpContext, string[]? codes)
        {
            return await Task.FromResult(true);
        }
    }
}
