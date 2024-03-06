using Microsoft.AspNetCore.Authorization;

namespace Tenon.AspNetCore.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeScopeAttribute(string[] codes) : AuthorizeAttribute
{
    public string[] Codes { get; set; } = codes;
}