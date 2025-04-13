using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalavisorApi.Services;
using GalavisorApi.Models;
using Microsoft.AspNetCore.Authorization;

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

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<PlanetModel>>> getAllPlanets()
    {
        return Ok(await _planetService.GetAllPlanets());
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<PlanetModel>> getPlanetById(int id)
    {
        return Ok(await _planetService.GetPlanetById(id));
    }

    [AllowAnonymous]
    [HttpPost("weather/{id}")]
    public async Task<ActionResult<string>> getPlanetWeatherById(int id)
    {
        return Ok(await _planetService.GetPlanetWeatherById(id));
    }

    [AllowAnonymous]
    [HttpPost("add")]
    public async Task<ActionResult<PlanetModel>> addPlanet([FromBody] PlanetModel request)
    {
        return Ok(await _planetService.AddPlanet(request));
    }

    [AllowAnonymous]
    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<bool>> addPlanet(int id)
    {
        return Ok(await _planetService.DeletePlanet(id));
    }

    [AllowAnonymous]
    [HttpPatch("update/{id}")]
    public async Task<ActionResult<bool>> updatePlanet([FromBody] PlanetModel planet)
    {
        return Ok(await _planetService.UpdatePlanet(planet));
    }
}