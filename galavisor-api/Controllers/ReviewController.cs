using Microsoft.AspNetCore.Mvc;
using GalavisorApi.Models;
using GalavisorApi.Services;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("review")]
public class ReviewController : ControllerBase
{
    // private readonly ReviewService _service;

    // public ReviewController(ReviewService service)
    // {
    //     _service = service;
    // }

    [HttpGet("")]
    public JsonResult Get()
    {   
        return new JsonResult(Ok("Hello from review"));
    }

    [HttpPost("")]
    public JsonResult Post(Review request)
    {   
        return new JsonResult(Ok("Added comment: " + request.comment + " with a rating of " + request.rating.ToString()));
    }
}
