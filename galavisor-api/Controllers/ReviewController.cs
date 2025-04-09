using Microsoft.AspNetCore.Mvc;
using GalavisorApi.Services;
using GalavisorApi.Models;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("reviews")]
public class ReviewController : ControllerBase
{
    private readonly ReviewService reviewService;

    public ReviewController(ReviewService service)
    {
        reviewService = service;
    }

    [HttpPost]
    public ActionResult<ReviewModel> AddReview([FromBody] ReviewModel request)
    {
        var result = reviewService.AddReview(request.Rating, request.Comment);
        return CreatedAtAction(nameof(GetReview), new { id = result.ReviewId }, result);
    }

    [HttpGet]
    public ActionResult<List<ReviewModel>> GetAllReviews()
    {
        return Ok(reviewService.GetAllReviews());
    }

    [HttpGet("{id}")]
    public ActionResult<ReviewModel> GetReview(int id)
    {
        var review = reviewService.GetReviewById(id);
        return review != null ? Ok(review) : NotFound();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteReview(int id)
    {
        var deleted = reviewService.DeleteReview(id);
        return deleted ? NoContent() : NotFound();
    }
}