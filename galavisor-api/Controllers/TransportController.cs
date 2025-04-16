using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalavisorApi.Services;
using GalavisorApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransportController(TransportService transportService, AuthService authService) : ControllerBase
{
    private readonly TransportService _transportService = transportService;

    private readonly AuthService _authService = authService;

    [Authorize]
    [HttpGet("planet/{planetName}")]
    public async Task<ActionResult<List<TransportModel>>> GetTransportByPlanet(string planetName)
    {
        var transport = await _transportService.GetTransportByPlanet(planetName);
        return Ok(transport);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<object>> AddTransport([FromBody] TransportModel transport)
    {
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if(await _authService.IsSubAdmin(GoogleSubject)){
            var (newTransport, isNewlyCreated) = await _transportService.AddTransport(transport);
        
            // Build a response with status information
            var response = new 
            {
                transport = newTransport,
                status = new
                {
                    isNewlyCreated
                }
            };
            
            return StatusCode(201, response);
        } else {
            return StatusCode(403, new { error = "Failed", message = "You cannot access this command, only available to admins" });
        }
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateTransport([FromBody] TransportUpdateModel model)
    {
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if(await _authService.IsSubAdmin(GoogleSubject)){
            if (string.IsNullOrEmpty(model.CurrentName) || string.IsNullOrEmpty(model.NewName))
            {
                return BadRequest("Both current name and new name are required");
            }
            
            var success = await _transportService.UpdateTransport(model.CurrentName, model.NewName);
            if (!success)
                return NotFound($"Transport '{model.CurrentName}' not found");
            
            return Ok(new { message = $"Transport '{model.CurrentName}' updated to '{model.NewName}' successfully" });
        } else {
            return StatusCode(403, new { error = "Failed", message = "You cannot access this command, only available to admins" });
        }
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteTransport([FromBody] string transportName)
    {
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if(await _authService.IsSubAdmin(GoogleSubject)){
            if (string.IsNullOrEmpty(transportName))
            {
                return BadRequest("Transport name is required");
            }
            
            var success = await _transportService.DeleteTransport(transportName);
            if (!success)
                return NotFound($"Transport '{transportName}' not found");
            
            return Ok(new { message = $"Transport '{transportName}' deleted successfully" });
        } else {
            return StatusCode(403, new { error = "Failed", message = "You cannot access this command, only available to admins" });
        }
    }

    [Authorize]
    [HttpPost("link")]
    public async Task<ActionResult<object>> LinkTransportToPlanet([FromBody] TransportModel transport)
    {
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if(await _authService.IsSubAdmin(GoogleSubject)){
            if (string.IsNullOrEmpty(transport.Name) || string.IsNullOrEmpty(transport.PlanetName))
            {
                return BadRequest("Transport name and planet name are required");
            }
            
            try
            {
                bool isNewlyLinked = await _transportService.LinkTransportToPlanet(transport.Name, transport.PlanetName);
                
                // Build a response with status information
                var response = new 
                {
                    transport = new TransportModel 
                    { 
                        Name = transport.Name,
                        PlanetName = transport.PlanetName
                    },
                    status = new
                    {
                        isNewlyLinked
                    }
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        } else {
            return StatusCode(403, new { error = "Failed", message = "You cannot access this command, only available to admins" });
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<TransportModel>>> GetAllTransport()
    {
        var transport = await _transportService.GetAllTransport();
        return Ok(transport);
    }

}