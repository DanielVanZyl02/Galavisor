using GalavisorApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    private readonly AuthService _authService = authService;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthCodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AuthCode))
        {
            return BadRequest(new { message = "Authentication failed", error = "authCode is required" });
        }

        try
        {
            var jwtJson = await _authService.AuthenticateUserAsync(request.AuthCode);
            if(jwtJson != null){
                var User = await _authService.GetOrCreateUser(jwtJson);
                return Ok(new { message = "Success", jwt = jwtJson, user = User});
            } else{
                return BadRequest(new { message = "Authentication failed", error = "jwt returned from google was null"});
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Authentication failed", error = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("debug-jwt")]
    public IActionResult ShowAllClaims()
    {
        var claims = HttpContext.User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}

public class AuthCodeRequest
{
    public required string AuthCode { get; set; }
}
