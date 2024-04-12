using CleanArchitecture.Identity.Application.Dtos;
using CleanArchitecture.Identity.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tenon.AspNetCore.Controllers;
using Tenon.Models.Dtos;

namespace CleanArchitecture.Identity.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UpdateAsync([FromRoute] long id, [FromBody] UserUpdationDto input)
    {
        return Result(await _userService.UpdateAsync(id, input));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync([FromRoute] long id)
    {
        return Result(await _userService.DeleteAsync(id));
    }

    [HttpGet("page")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<UserDto>>> GetPagedAsync([FromQuery] UserSearchPagedDto search)
    {
        return await _userService.GetPagedAsync(search);
    }
}