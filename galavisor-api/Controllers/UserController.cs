using GalavisorApi.Services;
using Microsoft.AspNetCore.Mvc;
using GalavisorApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("users")]
public class UserController(UserService userService, AuthService authService) : ControllerBase
{
    private readonly UserService _userService = userService;
    private readonly AuthService _authService = authService;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<UserModel>>> GetAllUsers()
    {
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if(await _authService.IsSubAdmin(GoogleSubject)){
            return Ok(new { message = "Success", users = await _userService.GetAllUsers()});
        } else {
            return StatusCode(403, new { message = "Get failed", error = "You cannot access this command, only available to admins" });
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserModel>> GetUser(int id)
    {
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if(await _authService.IsSubAdmin(GoogleSubject)){
            var User = await _userService.GetUser(id);
            return User != null ? Ok(new { message = "Success", user = User}) : NotFound();
        } else {
            return StatusCode(403, new { message = "Get failed", error = "You cannot access this command, only available to admins" });
        }    
    }

    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> UpdateUserConfig([FromBody] UpdateRequest request)
    {
        try {
            if(request.HomePlanet != null && request.Username != null){
                var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
                return Ok(new { message = "Success", user = await _userService.UpdateUserConfig(GoogleSubject, request.HomePlanet, request.Username)});
            } else{
                return BadRequest(new { message = "Update failed", error = "request body is required to perform update" });
            }
        } catch (Exception ex)
        {
            return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
        }
    }

    [Authorize]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateStatusOrRole(int id, [FromBody] UpdateRequest request)
    {
        try {
            if(request.Active != null){
                var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
                if(await _authService.IsSubAdmin(GoogleSubject) && id != -1){
                    return Ok(new { message = "Success", user = await _userService.UpdateActiveStatusById(request.Active ?? true, id)});
                } else{
                    return Ok(new { message = "Success", user = await _userService.UpdateActiveStatusBySub(request.Active ?? true, GoogleSubject)});
                }
            } else if(request.Role != null){
                var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
                if(await _authService.IsSubAdmin(GoogleSubject)){
                    return Ok(new { message = "Success", user = await _userService.UpdateRole(request.Role ?? "", id)});
                } else{
                    return StatusCode(403, new { message = "Update failed", error = "You cannot access this command, only available to admins" });
                }
            } else{
                return BadRequest(new { message = "Update failed", error = "request body is required to perform update" });
            }
        } catch (Exception ex)
        {
            return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
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

