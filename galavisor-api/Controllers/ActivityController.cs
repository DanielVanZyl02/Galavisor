using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalavisorApi.Services;
using GalavisorApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivityController(ActivityService activityService, AuthService authService) : ControllerBase
{
    private readonly ActivityService _activityService = activityService;

    private readonly AuthService _authService = authService;

    [Authorize]
    [HttpGet("planet/{planetName}")]
    public async Task<ActionResult<List<ActivityModel>>> GetActivitiesByPlanet(string planetName)
    {
        var activities = await _activityService.GetActivitiesByPlanet(planetName);
        return Ok(activities);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<object>> AddActivity([FromBody] ActivityModel activity)
    {
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if(await _authService.IsSubAdmin(GoogleSubject)){
            var (newActivity, isNewlyCreated) = await _activityService.AddActivity(activity);
        
            // Build a response with status information
            var response = new 
            {
                activity = newActivity,
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
    public async Task<IActionResult> UpdateActivity([FromBody] ActivityUpdateModel model)
    {
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if(await _authService.IsSubAdmin(GoogleSubject)){
            if (string.IsNullOrEmpty(model.CurrentName) || string.IsNullOrEmpty(model.NewName))
            {
                return BadRequest("Both current name and new name are required");
            }
            
            var success = await _activityService.UpdateActivity(model.CurrentName, model.NewName);
            if (!success)
                return NotFound($"Activity '{model.CurrentName}' not found");
            
            return Ok(new { message = $"Activity '{model.CurrentName}' updated to '{model.NewName}' successfully" });
        } else {
            return StatusCode(403, new { error = "Failed", message = "You cannot access this command, only available to admins" });
        }
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteActivity([FromBody] string activityName)
    {
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if(await _authService.IsSubAdmin(GoogleSubject)){
            if (string.IsNullOrEmpty(activityName))
            {
                return BadRequest("Activity name is required");
            }
            
            var success = await _activityService.DeleteActivity(activityName);
            if (!success)
                return NotFound($"Activity '{activityName}' not found");
            
            return Ok(new { message = $"Activity '{activityName}' deleted successfully" });
        } else {
            return StatusCode(403, new { error = "Failed", message = "You cannot access this command, only available to admins" });
        }
    }

    [Authorize]
    [HttpPost("link")]
    public async Task<ActionResult<object>> LinkActivityToPlanet([FromBody] ActivityModel activity)
    {
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if(await _authService.IsSubAdmin(GoogleSubject)){
            if (string.IsNullOrEmpty(activity.Name) || string.IsNullOrEmpty(activity.PlanetName))
            {
                return BadRequest("Activity name and planet name are required");
            }
            
            try
            {
                bool isNewlyLinked = await _activityService.LinkActivityToPlanet(activity.Name, activity.PlanetName);
                
                // Build a response with status information
                var response = new 
                {
                    activity = new ActivityModel 
                    { 
                        Name = activity.Name,
                        PlanetName = activity.PlanetName
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
    public async Task<ActionResult<List<ActivityModel>>> GetAllActivities()
    {
        var activity = await _activityService.GetAllActivities();
        return Ok(activity);
    }
} 