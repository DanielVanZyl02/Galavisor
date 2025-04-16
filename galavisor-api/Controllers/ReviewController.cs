using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

using GalavisorApi.Services;
using GalavisorApi.Models;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("reviews")]
public class ReviewController : ControllerBase
{
    private readonly ReviewService _reviewService;
    private readonly AuthService _authService;
    private readonly PlanetService _planetService;
    private readonly UserService _userService;
    public ReviewController(ReviewService reviewService, AuthService authService, PlanetService planetService, UserService userService)
    {
        _reviewService = reviewService;
        _authService = authService;
        _planetService = planetService;
        _userService = userService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ReviewReturnModel>> AddReview([FromBody] ReviewModel request)
    {
        string token = "";
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = authHeader.Substring("Bearer ".Length).Trim();   
        }

        request.UserId = await _authService.GetLoggedInUser(token);

        if(request.Rating < 1 || request.Rating > 5)
        {
            return StatusCode(403, new { error = "Get failed", message = "You can only post reviews with a rating between 1 and 5" });
        }

        var review = await _reviewService.AddReview(request);

        var planet = await _planetService.GetPlanetById(review.PlanetId.Value);
        var user = await _userService.GetUser(review.UserId.Value);

        ReviewReturnModel reviewResponse = new ReviewReturnModel
        {
            ReviewId = review.ReviewId,
            PlanetName = planet.Name,
            UserName = user.Name,
            Rating = review.Rating,
            Comment = review.Comment
        };
    
        return Ok(new {status = "Success", review = reviewResponse});
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewReturnModel>>> GetReviews(
        [FromQuery] bool posted,
        [FromQuery] int? ratingEq,
        [FromQuery] int? ratingGte,
        [FromQuery] int? ratingLte)
    {
        try
        {
            int userId = -1;
            if(posted){
                string token = "";
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();   
                }

                userId = await _authService.GetLoggedInUser(token);                
            }

            var reviewResponse = await _reviewService.GetAllReviews(userId,ratingEq, ratingGte, ratingLte);

            return Ok(new {status = "Success", reviews = reviewResponse});
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error");
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewReturnModel>> GetReview(int id)
    {
        try
        {
            var review = await _reviewService.GetReviewById(id);
            Console.WriteLine(review.PlanetName);
            if (review == null)
            {
                return NotFound($"Review with ID {id} not found");
            }
            return Ok(new {status = "Success", reviews = review});
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error");
        }
    }

    [Authorize]
    [HttpGet("planets/{planetId}")]
    public async Task<ActionResult<IEnumerable<ReviewReturnModel>>> GetReviewsByPlanet(
        int planetId,
        [FromQuery] bool posted,
        [FromQuery] int? ratingEq,
        [FromQuery] int? ratingGte,
        [FromQuery] int? ratingLte)
    {
        try
        {
            int userId = -1;
            if(posted)
            {
                string token = "";
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();   
                }

                userId = await _authService.GetLoggedInUser(token);                
            }
            var reviews = await _reviewService.GetReviewByPlanetId(planetId, userId, ratingEq, ratingGte, ratingLte);
                        List<ReviewReturnModel> reviewResponse = new List<ReviewReturnModel>();
            
            foreach (var review in reviews)
            {
                if (review.PlanetId.HasValue && review.UserId.HasValue)
                {
                    var planet = await _planetService.GetPlanetById(review.PlanetId.Value);
                    var user = await _userService.GetUser(review.UserId.Value);


                    ReviewReturnModel mappedReview = new ReviewReturnModel
                    {
                        ReviewId = review.ReviewId,
                        PlanetName = planet.Name,
                        UserName = user.Name,
                        Rating = review.Rating,
                        Comment = review.Comment
                    };

                    reviewResponse.Add(mappedReview);
                }
            }

            return Ok(new {status = "Success", reviews = reviewResponse});
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error");
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewModel request)
    {
        request.ReviewId = id;
        string token = "";
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();

        if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = authHeader.Substring("Bearer ".Length).Trim();   
        }

        var loggedInUser = await _authService.GetLoggedInUser(token);
        bool isLoggedInUser = false;

        if(request.Rating < 1 || request.Rating > 5)
        {
            return StatusCode(403, new { error = "Get failed", message = "You can only post reviews with a rating between 1 and 5" });
        }

        if(loggedInUser != -1)
        {
            var review = await _reviewService.GetReviewById(id);
            
            if(review == null)
            {
                return  Ok(new {status = "Fail", message = $"Review {id} not found" });
            }
            
            if(review.UserId == loggedInUser)
            {
                isLoggedInUser = true;
            }
        }

        if(await _authService.IsSubAdmin(GoogleSubject) || isLoggedInUser)
        {
            var updatedReview = await _reviewService.UpdateReview(request);
            if (updatedReview == null)
                return NotFound();

            var planet = await _planetService.GetPlanetById(updatedReview.PlanetId.Value);
            var user = await _userService.GetUser(updatedReview.UserId.Value);

            ReviewReturnModel reviewResponse = new ReviewReturnModel
            {
                ReviewId = updatedReview.ReviewId,
                PlanetName = planet.Name,
                UserName = user.Name,
                Rating = updatedReview.Rating,
                Comment = updatedReview.Comment
            };    

            return Ok(new {status = "Success", review = reviewResponse});
        } 
        else
        {
            return StatusCode(403, new { error = "Get failed", message = "You can only edit reviews you posted" });
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {   
        string token = "";
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();

        if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = authHeader.Substring("Bearer ".Length).Trim();   
        }

        var loggedInUser = await _authService.GetLoggedInUser(token);
        bool isLoggedInUser = false;

        if(loggedInUser != -1)
        {
            var review = await _reviewService.GetReviewById(id);

            if(review == null)
            {
                return  Ok(new {status = "Fail", message = $"Review {id} not found" });
            }

            if(review.UserId == loggedInUser)
            {
                isLoggedInUser = true;
            }
        }

        if(await _authService.IsSubAdmin(GoogleSubject) || isLoggedInUser)
        {
            var deleted = await _reviewService.DeleteReview(id);
            return deleted ? Ok(new { status = "Success", message = $"Review {id} succesfully deleted" }): Ok(new {status = "Fail", message = $"Review {id} not found" });
        } else
        {
            return StatusCode(403, new { error = "Get failed", message = "You can only delete reviews you posted" });
        }
    }
}