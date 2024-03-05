using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Tenon.AspNetCore.Authentication.Basic;
using Tenon.AspNetCore.Authentication.Bearer;

namespace Tenon.AspNetCore.Authorization.Bearer;

public abstract class BearAuthorizationHandler : AuthorizationHandler<BearRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        BearRequirement requirement)
    {
        if (context.User.Identity is not null && context.User.Identity.IsAuthenticated &&
            context.Resource is HttpContext httpContext)
        {
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authHeader) &&
                authHeader.StartsWith(BasicDefaults.AuthenticationScheme,
                    StringComparison.OrdinalIgnoreCase))
            {
                var codes = httpContext.GetEndpoint()?.Metadata?.GetMetadata<BearerAuthorizeAttribute>()?.Codes;
                if (!await CheckUserPermissionsAsync(httpContext, codes))
                    context.Fail();
                else
                    context.Succeed(requirement);
            }
            return;
        }

        context.Fail();
    }

    protected abstract Task<bool> CheckUserPermissionsAsync(HttpContext httpContext, string[]? codes);
}