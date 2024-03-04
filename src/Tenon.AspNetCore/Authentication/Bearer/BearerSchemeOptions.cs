using Microsoft.AspNetCore.Authentication;
using Tenon.AspNetCore.Authentication.Bearer.Jwt;

namespace Tenon.AspNetCore.Authentication.Bearer
{
    public class BearerSchemeOptions : AuthenticationSchemeOptions
    {
        public Func<BearerTokenValidatedContext, Task>? OnTokenValidated { get; set; }
    }
}
