using Microsoft.AspNetCore.Authorization;

namespace Tenon.AspNetCore.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeScopeAttribute : AuthorizeAttribute
{
    public AuthorizeScopeAttribute(string[] codes, string? policyName = null)
    {
        Codes = codes;
        Policy = string.IsNullOrEmpty(policyName) ? AuthorizePolicy.Default : policyName;
    }
    public string[] Codes { get; set; }
}