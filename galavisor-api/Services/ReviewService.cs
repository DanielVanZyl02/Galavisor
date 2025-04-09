using GalavisorApi.Models;
using GalavisorApi.Repositories;

namespace GalavisorApi.Services;
public class ReviewService
{
    private readonly ReviewRepository reviewRepository;

    public ReviewService(ReviewRepository repo)
    {
        reviewRepository = repo;
    }

    public ReviewModel AddReview(int rating, string? comment)
    {
        var review = reviewRepository.Add(rating, comment);
        return MapToModel(review);
    }

    public List<ReviewModel> GetAllReviews()
    {
        return reviewRepository.GetAll().Select(MapToModel).ToList();
    }

    public ReviewModel? GetReviewById(int id)
    {
        var review = reviewRepository.GetById(id);
        return review != null ? MapToModel(review) : null;
    }

    public bool DeleteReview(int id)
    {
        return reviewRepository.Delete(id);
    }

    private ReviewModel MapToModel(ReviewModel review)
    {
        return new ReviewModel
        {
            ReviewId = review.ReviewId,
            Rating = review.Rating,
            Comment = review.Comment
        };
    }
}
