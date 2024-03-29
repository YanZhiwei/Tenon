using CleanArchitecture.Identity.Application.Dtos;
using CleanArchitecture.Identity.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tenon.AspNetCore.Controllers;

namespace CleanArchitecture.Identity.Api.Controllers;

public sealed class AccountController(IUserService userService) : AbstractController
{
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserTokenInfoDto>> LoginAsync([FromBody] UserLoginDto input)
    {
        var result = await userService.LoginAsync(input);
        if (result.IsSuccess)
        {
            var validatedInfo = result.Content;
            return Created("/auth/session", null);
        }

        return Problem(result.ProblemDetails);
    }
}