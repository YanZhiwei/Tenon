using Microsoft.AspNetCore.Authorization;
using Tenon.AspNetCore.Authentication.Bearer;

namespace Tenon.AspNetCore.Authorization.Bearer;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BearerAuthorizeAttribute : AuthorizeAttribute
{
    public BearerAuthorizeAttribute(string[] codes)
    {
        Codes = codes;
        Policy = AuthorizePolicy.Default;
        AuthenticationSchemes = BearerAuthenticationHandler.AuthenticationScheme;
    }

    public string[] Codes { get; set; }
}