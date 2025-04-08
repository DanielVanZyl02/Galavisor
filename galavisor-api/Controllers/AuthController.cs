using GalavisorApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthCodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AuthCode))
        {
            return BadRequest(new { message = "authCode is required" });
        }

        try
        {
            var jwtJson = await _authService.AuthenticateUserAsync(request.AuthCode);
            return Ok(jwtJson);
        }
        catch (Exception ex)
        {
            // Log ex.Message if needed
            Console.WriteLine(ex.Message);
            return StatusCode(500, new { message = "Authentication failed", error = ex.Message });
        }
    }
}

public class AuthCodeRequest
{
    public required string AuthCode { get; set; }
}
