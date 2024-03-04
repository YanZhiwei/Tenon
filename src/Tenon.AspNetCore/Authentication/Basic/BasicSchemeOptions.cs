using Microsoft.AspNetCore.Authentication;

namespace Tenon.AspNetCore.Authentication.Basic;

public class BasicSchemeOptions : AuthenticationSchemeOptions
{
    public Func<BasicTokenValidatedContext, Task>? OnTokenValidated { get; set; }

}