using CleanArchitecture.Identity.Application.Dtos;
using CleanArchitecture.Identity.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tenon.AspNetCore.Controllers;
using Tenon.FluentValidation.AspNetCore.Extensions;

namespace CleanArchitecture.Identity.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AccountController(IUserService userService, IValidator<UserLoginDto> validator) : AbstractController
{
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserTokenInfoDto>> LoginAsync([FromBody] UserLoginDto input)
    {
        //https://lurumad.github.io/problem-details-an-standard-way-for-specifying-errors-in-http-api-responses-asp.net-core
        var validationResult = await validator.ValidateAsync(input);
        if (!validationResult.IsValid)
            return Problem(validationResult.ToFluentValidationProblemDetails());
        var result = await userService.LoginAsync(input);
        if (result.Succeeded)
        {
            var validatedInfo = result.Content;
            return Created("/auth/session", null);
        }

        return Problem(result.ProblemDetails);
    }
}