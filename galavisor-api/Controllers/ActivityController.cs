using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalavisorApi.Services;
using GalavisorApi.Models;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("activities")]
public class ActivityController : ControllerBase
{
    private readonly ActivityService _activityService;

    public ActivityController(ActivityService service)
    {
        _activityService = service;
    }

    [HttpGet("planets/{planetId}")]
    public async Task<ActionResult<List<ActivityModel>>> GetActivitiesByPlanet(int planetId)
    {
        var activities = await _activityService.GetActivitiesByPlanet(planetId);
        return Ok(activities);
    }

    [HttpPost]
    public async Task<ActionResult<ActivityModel>> AddActivity([FromBody] ActivityModel activity)
    {
        var result = await _activityService.AddActivity(activity);
        return CreatedAtAction(nameof(GetActivitiesByPlanet), new { planetId = result.ActivityId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateActivity(int id, [FromBody] ActivityModel activity)
    {
        if (id != activity.ActivityId)
            return BadRequest();
            
        var updated = await _activityService.UpdateActivity(activity);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivity(int id)
    {
        var deleted = await _activityService.DeleteActivity(id);
        return deleted ? NoContent() : NotFound();
    }
} 