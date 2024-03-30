using CleanArchitecture.Identity.Application.Dtos;
using CleanArchitecture.Identity.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Tenon.AspNetCore.Controllers;

namespace CleanArchitecture.Identity.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : AbstractController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<long>> CreateAsync([FromBody] UserCreationDto input)
    {
        return CreatedResult(await _userService.CreateAsync(input));
    }
}