using GalavisorApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AuthService authService, UserService userService) : ControllerBase
{
    private readonly AuthService _authService = authService;
    private readonly UserService _userService = userService;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthCodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AuthCode))
        {
            return BadRequest(new { message = "Authentication failed", error = "authCode is required" });
        }

        // try
        // {
            var jwt = await _authService.AuthenticateUserAsync(request.AuthCode);
            if(jwt != null){
                var User = await _authService.GetOrCreateUser(jwt);
                User = await _userService.UpdateActiveStatusBySub(true, User.GoogleSubject);
                return Ok(new { message = "Success", jwt, user = User});
            } else{
                return BadRequest(new { message = "Authentication failed", error = "jwt returned from google was null"});
            }
        // }
        // catch (Exception ex)
        // {
        //     return StatusCode(500, new { message = "Authentication failed", error = ex.Message });
        // }
    }
}

public class AuthCodeRequest
{
    public required string AuthCode { get; set; }
}
