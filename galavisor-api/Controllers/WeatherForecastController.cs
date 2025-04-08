using Microsoft.AspNetCore.Mvc;
using GalavisorApi.Models;
using GalavisorApi.Services;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("api")]
public class WeatherForecastController : ControllerBase
{
    private readonly WeatherForecastService _service;

    public WeatherForecastController(WeatherForecastService service)
    {
        _service = service;
    }

    [HttpGet("weather")]
    public IEnumerable<WeatherForecast> Get()
    {
        return _service.GetForecast();
    }
}
