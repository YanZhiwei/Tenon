using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Tenon.AspNetCore.Authentication.Bearer
{
    public class BearerTokenValidatedContext(
        HttpContext context,
        AuthenticationScheme scheme,
        BearerSchemeOptions options)
        : ResultContext<BearerSchemeOptions>(context, scheme, options);
}
