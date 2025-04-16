using GalavisorApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AuthService AuthService, UserService UserService) : ControllerBase
{
    private readonly AuthService _authService = AuthService;
    private readonly UserService _userService = UserService;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthCodeRequest Request)
    {
        if (string.IsNullOrWhiteSpace(Request.AuthCode))
        {
            return BadRequest(new { error = "Authentication failed", message = "authCode is required" });
        } else{
            try
            {
                var Jwt = await _authService.AuthenticateUserAsync(Request.AuthCode);
                if(Jwt != null){
                    var User = await _authService.GetOrCreateUser(Jwt);
                    User = await _userService.UpdateActiveStatusBySub(true, User.GoogleSubject);
                    return Ok(new { message = "Success", jwt = Jwt, user = User});
                } else{
                    return BadRequest(new { error = "Authentication failed", message = "Jwt returned from google was null"});
                }
            }
            catch (Exception Error)
            {
                return StatusCode(500, new { error = "Authentication failed", message = Error.Message });
            }

        }
    }
}

public class AuthCodeRequest
{
    public required string AuthCode { get; set; }
}
