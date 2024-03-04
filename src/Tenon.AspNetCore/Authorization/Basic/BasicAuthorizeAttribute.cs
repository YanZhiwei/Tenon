using Microsoft.AspNetCore.Authorization;
using Tenon.AspNetCore.Authentication.Basic;

namespace Tenon.AspNetCore.Authorization.Basic;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BasicAuthorizeAttribute : AuthorizeAttribute
{
    public BasicAuthorizeAttribute(string[] codes)
    {
        Codes = codes;
        Policy = AuthorizePolicy.Default;
        AuthenticationSchemes = BasicAuthenticationHandler.AuthenticationScheme;
    }

    public string[] Codes { get; set; }
}