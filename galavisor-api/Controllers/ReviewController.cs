using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalavisorApi.Services;
using GalavisorApi.Models;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("reviews")]
public class ReviewController : ControllerBase
{
    private readonly ReviewService _reviewService;

    public ReviewController(ReviewService service)
    {
        _reviewService = service;
    }

    [HttpPost]
    public async Task<ActionResult<ReviewModel>> AddReview([FromBody] ReviewModel request)
    {
        var result = await _reviewService.AddReview(request);
        return CreatedAtAction(nameof(GetReview), new { id = result.ReviewId }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<ReviewModel>>> GetAllReviews()
    {
        return Ok(await _reviewService.GetAllReviews());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewModel>> GetReview(int id)
    {
        var review = await _reviewService.GetReviewById(id);
        return review != null ? Ok(review) : NotFound();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewModel request)
    {
        if (id != request.ReviewId)
            return BadRequest();
            
        var updated = await _reviewService.UpdateReview(request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var deleted = await _reviewService.DeleteReview(id);
        return deleted ? NoContent() : NotFound();
    }
}