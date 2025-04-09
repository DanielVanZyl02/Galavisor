using GalavisorApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace GalavisorApi.Repositories;
public class ReviewRepository
{
    private readonly List<ReviewModel> _reviews = new();
    private int _nextId = 1;

    public ReviewModel Add(int rating, string? comment = null)
    {
        var review = new ReviewModel
        {
            ReviewId = _nextId++,
            Rating = rating,
            Comment = comment
        };
        _reviews.Add(review);
        return review;
    }

    public ReviewModel? GetById(int id) => _reviews.FirstOrDefault(r => r.ReviewId == id);

    public List<ReviewModel> GetAll() => _reviews.ToList();

    public bool Update(int id, int? rating = null, string? comment = null)
    {
        var review = GetById(id);
        if (review == null) return false;

        if (rating.HasValue) review.Rating = rating.Value;
        if (comment != null) review.Comment = comment;

        return true;
    }

    public bool Delete(int id)
    {
        var review = GetById(id);
        if (review == null) return false;
        _reviews.Remove(review);
        return true;
    }
}