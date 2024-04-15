using Microsoft.AspNetCore.Authorization;

namespace Tenon.AspNetCore.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeScopeAttribute : AuthorizeAttribute
{
    public AuthorizeScopeAttribute(string[] codes)
    {
        Codes = codes;
        Policy = AuthorizePolicy.Default;
    }
    public string[] Codes { get; set; }
}