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
    public async Task<ActionResult<IEnumerable<ReviewModel>>> GetReviews(
        [FromQuery] int? ratingEq,
        [FromQuery] int? ratingGte,
        [FromQuery] int? ratingLte)
    {
        try
        {
            var reviews = await _reviewService.GetAllReviews(ratingEq, ratingGte, ratingLte);
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewModel>> GetReview(int id)
    {
        try
        {
            var review = await _reviewService.GetReviewById(id);
            if (review == null)
            {
                return NotFound($"Review with ID {id} not found");
            }
            return Ok(review);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("planets/{planetId}")]
    public async Task<ActionResult<IEnumerable<ReviewModel>>> GetReviewsByPlanet(
        int planetId,
        [FromQuery] int? ratingEq,
        [FromQuery] int? ratingGte,
        [FromQuery] int? ratingLte)
    {
        try
        {
            var reviews = await _reviewService.GetReviewByPlanetId(planetId, ratingEq, ratingGte, ratingLte);
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewModel request)
    {
        request.ReviewId = id;
        
        var updatedReview = await _reviewService.UpdateReview(request);
        
        if (updatedReview == null)
            return NotFound();
            
        return Ok(updatedReview);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var deleted = await _reviewService.DeleteReview(id);
        return deleted ? Ok($"Review {id} succesfully deleted") : NotFound($"Review {id} not found") ;
    }
}