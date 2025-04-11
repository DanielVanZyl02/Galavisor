using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GalavisorApi.Models;
using GalavisorApi.Repositories;

namespace GalavisorApi.Services;
public class ReviewService
{
    private readonly ReviewRepository _reviewRepository;

    public ReviewService(ReviewRepository repo)
    {
        _reviewRepository = repo;
    }

    public async Task<ReviewModel> AddReview(ReviewModel review)
    {
        var addedReview = await _reviewRepository.Add(review);
        return addedReview;
    }

    public async Task<List<ReviewModel>> GetAllReviews()
    {
        return await _reviewRepository.GetAll();
    }

    public async Task<ReviewModel?> GetReviewById(int id)
    {
        return await _reviewRepository.GetById(id);
    }
    public async Task<List<ReviewModel>> GetReviewByPlanetId(int planetId)
    {
        return await _reviewRepository.GetByPlanetId(planetId);
    }
    public async Task<bool> UpdateReview(ReviewModel review)
    {
        return await _reviewRepository.Update(review);
    }

    public async Task<bool> DeleteReview(int id)
    {
        return await _reviewRepository.Delete(id);
    }
}
