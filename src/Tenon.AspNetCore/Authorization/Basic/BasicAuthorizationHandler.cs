using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Tenon.AspNetCore.Authentication.Basic;

namespace Tenon.AspNetCore.Authorization.Basic;

public abstract class BasicAuthorizationHandler : AuthorizationHandler<BasicRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        BasicRequirement requirement)
    {
        if (context.User.Identity is not null && context.User.Identity.IsAuthenticated &&
            context.Resource is HttpContext httpContext)
        {
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authHeader) &&
                authHeader.StartsWith(BasicDefaults.AuthenticationScheme,
                    StringComparison.OrdinalIgnoreCase))
            {
                var codes = httpContext.GetEndpoint()?.Metadata?.GetMetadata<BasicAuthorizeAttribute>()?.Codes;
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