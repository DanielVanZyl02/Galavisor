using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalavisorApi.Services;
using GalavisorApi.Models;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("planets")]

public class PlanetController : ControllerBase
{
    private readonly PlanetService _planetService;

    public PlanetController(PlanetService service)
    {
        _planetService = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<PlanetModel>>> getAllPlanets()
    {
        return Ok(await _planetService.GetAllPlanets());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<List<PlanetModel>>> getPlanetById(int id)
    {
        return Ok(await _planetService.GetPlanetById(id));
    }

    [HttpPost("weather/{id}")]
    public async Task<ActionResult<string>> getPlanetWeatherById(int id)
    {   
        return Ok(await _planetService.GetPlanetWeatherById(id));
    }

    [HttpPost("add")]
    public async Task<ActionResult<PlanetModel>> addPlanet([FromBody] PlanetModel request)
    {   
        return Ok(await _planetService.AddPlanet(request));
    }
}