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

    public async Task<List<ReviewModel>> GetAllReviews(int userId, int? ratingEq = null, int? ratingGte = null, int? ratingLte = null)
    {
        if(userId == -1){
            return await _reviewRepository.GetAll(ratingEq, ratingGte, ratingLte);
        }else{
            return await _reviewRepository.GetAll(ratingEq, ratingGte, ratingLte, userId);
        }
        
    }

    public async Task<ReviewModel?> GetReviewById(int id)
    {
        return await _reviewRepository.GetById(id);
    }
    public async Task<List<ReviewModel>> GetReviewByPlanetId(int planetId, int userId, int? ratingEq = null, int? ratingGte = null, int? ratingLte = null)
    {
        if(userId == -1){
            return await _reviewRepository.GetByPlanetId(planetId, ratingEq, ratingGte, ratingLte);
        }else{
            return await _reviewRepository.GetByPlanetId(planetId, ratingEq, ratingGte, ratingLte, userId);
        }
        
    }
    public async Task<ReviewModel> UpdateReview(ReviewModel review)
    {
        return await _reviewRepository.Update(review);
    }

    public async Task<bool> DeleteReview(int id)
    {
        return await _reviewRepository.Delete(id);
    }
}
