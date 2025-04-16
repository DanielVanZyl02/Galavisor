using GalavisorApi.Services;
using Microsoft.AspNetCore.Mvc;
using GalavisorApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("users")]
public class UserController(UserService UserService, AuthService AuthService) : ControllerBase
{
    private readonly UserService _userService = UserService;
    private readonly AuthService _authService = AuthService;

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
    [HttpGet("{Id}")]
    public async Task<ActionResult<UserModel>> GetUser(int Id)
    {
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if(await _authService.IsSubAdmin(GoogleSubject)){
            var User = await _userService.GetUser(Id);
            return User != null ? Ok(new { message = "Success", user = User}) : NotFound();
        } else {
            return StatusCode(403, new { message = "Get failed", error = "You cannot access this command, only available to admins" });
        }    
    }

    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> UpdateUserConfig([FromBody] UpdateRequest Request)
    {
        try {
            if(Request.Username != null){
                var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
                return Ok(new { message = "Success", user = await _userService.UpdateUserConfig(GoogleSubject, Request.Username)});
            } else{
                return BadRequest(new { message = "Update failed", error = "Request body is required to perform update" });
            }
        } catch (Exception ex)
        {
            return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
        }
    }

    [Authorize]
    [HttpPatch("{Id}")]
    public async Task<IActionResult> UpdateStatusOrRole(int Id, [FromBody] UpdateRequest Request)
    {
        try {
            if(Request.Active != null){
                var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
                if(await _authService.IsSubAdmin(GoogleSubject) && Id != -1){
                    return Ok(new { message = "Success", user = await _userService.UpdateActiveStatusById(Request.Active ?? true, Id)});
                } else{
                    return Ok(new { message = "Success", user = await _userService.UpdateActiveStatusBySub(Request.Active ?? true, GoogleSubject)});
                }
            } else if(Request.Role != null){
                var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
                if(await _authService.IsSubAdmin(GoogleSubject)){
                    return Ok(new { message = "Success", user = await _userService.UpdateRole(Request.Role ?? "", Id)});
                } else{
                    return StatusCode(403, new { message = "Update failed", error = "You cannot access this command, only available to admins" });
                }
            } else{
                return BadRequest(new { message = "Update failed", error = "Request body is required to perform update" });
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
    public bool? Active { get; set; }
    public string? Role { get; set; }
}

