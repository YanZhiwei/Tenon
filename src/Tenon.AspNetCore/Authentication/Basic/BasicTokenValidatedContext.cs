using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Tenon.AspNetCore.Authentication.Basic
{
    public sealed class BasicTokenValidatedContext(
        HttpContext context,
        AuthenticationScheme scheme,
        BasicSchemeOptions options)
        : ResultContext<BasicSchemeOptions>(context, scheme, options);
}

