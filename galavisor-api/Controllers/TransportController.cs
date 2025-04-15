using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalavisorApi.Services;
using GalavisorApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransportController : ControllerBase
{
    private readonly TransportService _transportService;

    public TransportController(TransportService transportService)
    {
        _transportService = transportService;
    }

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
    }

    [Authorize]
    [HttpPut("{currentName}")]
    public async Task<IActionResult> UpdateTransport(string currentName, [FromBody] string newName)
    {
        var success = await _transportService.UpdateTransport(currentName, newName);
        if (!success)
            return NotFound();
        
        return Ok(new { message = $"Transport '{currentName}' updated to '{newName}' successfully" });
    }

    [Authorize]
    [HttpDelete("{transportName}")]
    public async Task<IActionResult> DeleteTransport(string transportName)
    {
        var success = await _transportService.DeleteTransport(transportName);
        if (!success)
            return NotFound();
        
        return Ok(new { message = $"Transport '{transportName}' deleted successfully" });
    }

    [Authorize]
    [HttpPost("link")]
    public async Task<ActionResult<object>> LinkTransportToPlanet([FromBody] TransportModel transport)
    {
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
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<TransportModel>>> GetAllTransport()
    {
        var transport = await _transportService.GetAllTransport();
        return Ok(transport);
    }

}