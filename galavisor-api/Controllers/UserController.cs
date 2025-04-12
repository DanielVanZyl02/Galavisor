using GalavisorApi.Services;
using Microsoft.AspNetCore.Mvc;
using GalavisorApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("users")]
public class UserController(UserService userService) : ControllerBase
{
    private readonly UserService _userService = userService;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<UserModel>>> GetAllUsers()
    {
        return Ok(await _userService.GetAllUsers());
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserModel>> GetUser(int id)
    {
        var User = await _userService.GetUser(id);
        return User != null ? Ok(User) : NotFound();
    }

    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> UpdateUserConfig([FromBody] UpdateRequest request)
    {
        if(request.HomePlanet != null && request.Username != null){
            return Ok();
        } else{
            return BadRequest(new { message = "Update failed", error = "request body is required to perform update" });
        }
    }

    [Authorize]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateStatusOrRole(int id, [FromBody] UpdateRequest request)
    {
        if(request.Active != null){
            return Ok();
        } else if(request.Role != null){
            return Ok();
        } else{
            return BadRequest(new { message = "Update failed", error = "request body is required to perform update" });
        }
    }
}

public class UpdateRequest
{
    public string? Username { get; set; }
    public string? HomePlanet { get; set; }
    public bool? Active { get; set; }
    public string? Role { get; set; }
}

