using Microsoft.AspNetCore.Authentication;

namespace Tenon.AspNetCore.Authentication.Bearer;

public class BearerSchemeOptions : AuthenticationSchemeOptions
{
    public Func<BearerTokenValidatedContext, Task>? OnTokenValidated { get; set; }
}