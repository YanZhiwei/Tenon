using Microsoft.AspNetCore.Authorization;
using Tenon.AspNetCore.Authentication.Basic;

namespace Tenon.AspNetCore.Authorization.Bearer;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BearerAuthorizeAttribute : AuthorizeAttribute
{
    public BearerAuthorizeAttribute(string[] codes)
    {
        Codes = codes;
        Policy = AuthorizePolicy.Default;
        AuthenticationSchemes = BasicDefaults.AuthenticationScheme;
    }

    public string[] Codes { get; set; }
}