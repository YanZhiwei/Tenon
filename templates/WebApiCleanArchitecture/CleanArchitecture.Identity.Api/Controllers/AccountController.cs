using System.Net;
using CleanArchitecture.Identity.Application.Dtos;
using CleanArchitecture.Identity.Application.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tenon.AspNetCore.Controllers;
using Tenon.FluentValidation.AspNetCore.Extensions.Models;

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
            return Problem(validationResult);
        var result = await userService.LoginAsync(input);
        if (result.Succeeded)
        {
            var validatedInfo = result.Content;
            return Created("/auth/session", null);
        }

        return Problem(result.ProblemDetails);
    }

    [NonAction]
    private ObjectResult Problem(ValidationResult validationResult)
    {
        //var problemDetails = new ProblemDetails();
        //problemDetails.Status = StatusCodes.Status400BadRequest;
        //problemDetails.Title = "One or more validation errors occurred.";
        //problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

        //foreach (var error in validationResult.Errors)
        //{
        //    problemDetails.Extensions.Add(error.PropertyName, error.ErrorMessage);
        //}
        var problemDetails = new FluentValidationProblemDetails(validationResult.Errors
        )
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = (int)HttpStatusCode.BadRequest
        };
        return new BadRequestObjectResult(problemDetails);
    }
}

internal class FieldCodeMessage(object field, string code, string message)
{
    public string Code = code;
    public object Field = field;
    public string Message = message;
}