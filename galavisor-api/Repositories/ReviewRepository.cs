using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using System.Data;
using System.Text;
using GalavisorApi.Models;
using GalavisorApi.Data;


namespace GalavisorApi.Repositories;

public class ReviewRepository
{
    private readonly DatabaseConnection _db;

    public ReviewRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public async Task<ReviewModel> Add(ReviewModel review)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            INSERT INTO review (planetid, userid, rating, comment)
            VALUES (@PlanetId, @UserId, @Rating, @Comment)
            RETURNING reviewid";
        
        var parameters = new
        {
            review.PlanetId,
            review.UserId,
            review.Rating,
            review.Comment
        };

        var reviewId = await connection.ExecuteScalarAsync<int>(sql, parameters);
        review.ReviewId = reviewId;
        return review;
    }

    public virtual async Task<ReviewModel?> GetById(int id)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ReviewModel>(
            @"SELECT reviewid AS ReviewId, 
                    planetid AS PlanetId, 
                    userid AS UserId, 
                    rating AS Rating, 
                    comment AS Comment 
              FROM review 
              WHERE reviewid = @Id", 
            new { Id = id });
    }
    
    public async Task<List<ReviewModel>> GetByPlanetId(int planetId, int? ratingEq = null, int? ratingGte = null, int? ratingLte = null, int? userId = null)
    {
        using var connection = _db.CreateConnection();
        
        var queryBuilder = new StringBuilder(@"
            SELECT reviewid AS ReviewId, 
                   planetid AS PlanetId, 
                   userid AS UserId, 
                   rating AS Rating, 
                   comment AS Comment 
            FROM review 
            WHERE planetId = @PlanetId");
        
        var parameters = new DynamicParameters();
        parameters.Add("PlanetId", planetId);
        
        AddRatingFilters(queryBuilder, parameters, ratingEq, ratingGte, ratingLte, userId);
        
        var reviews = await connection.QueryAsync<ReviewModel>(queryBuilder.ToString(), parameters);
        return reviews.ToList();
    }


    public async Task<List<ReviewModel>> GetAll(int? ratingEq = null, int? ratingGte = null, int? ratingLte = null, int? userId = null)
    {
        using var connection = _db.CreateConnection();
        
        var queryBuilder = new StringBuilder(@"
            SELECT reviewid AS ReviewId, 
                   planetid AS PlanetId, 
                   userid AS UserId, 
                   rating AS Rating, 
                   comment AS Comment 
            FROM review");
        
        var parameters = new DynamicParameters();

        if (ratingEq.HasValue || ratingGte.HasValue || ratingLte.HasValue || userId.HasValue)
        {
            queryBuilder.Append(" WHERE 1=1");
            AddRatingFilters(queryBuilder, parameters, ratingEq, ratingGte, ratingLte, userId);
        }
        
        var reviews = await connection.QueryAsync<ReviewModel>(queryBuilder.ToString(), parameters);
        return reviews.ToList();
    }

    public async Task<ReviewModel> Update(ReviewModel review)
    {
        using var connection = _db.CreateConnection();
        
        var updateFields = new List<string>();
        var parameters = new DynamicParameters();

        parameters.Add("ReviewId", review.ReviewId);
        
        if (review.Rating.HasValue)
        {
            updateFields.Add("rating = @Rating");
            parameters.Add("Rating", review.Rating);
        }
        
        if (!string.IsNullOrEmpty(review.Comment))
        {
            updateFields.Add("comment = @Comment");
            parameters.Add("Comment", review.Comment);
        }
        
        if (updateFields.Count == 0)
            return null;
        
        var sql = $@"
            UPDATE review 
            SET {string.Join(", ", updateFields)}
            WHERE reviewid = @ReviewId;
            
            SELECT reviewid as ReviewId, 
                planetid as PlanetId, 
                userid as UserId, 
                rating as Rating, 
                comment as Comment
            FROM review
            WHERE reviewid = @ReviewId;";
        
        var updatedReview = await connection.QuerySingleOrDefaultAsync<ReviewModel>(sql, parameters);
        return updatedReview;
    }

    public async Task<bool> Delete(int id)
    {
        using var connection = _db.CreateConnection();
        var result = await connection.ExecuteAsync(
            "DELETE FROM review WHERE reviewid = @Id", new { Id = id });
        return result > 0;
    }

    private void AddRatingFilters(StringBuilder queryBuilder, DynamicParameters parameters, int? ratingEq, int? ratingGte, int? ratingLte, int? userId)
    {
        if (ratingEq.HasValue)
        {
            queryBuilder.Append(" AND rating = @RatingEq");
            parameters.Add("RatingEq", ratingEq.Value);
        }
        
        if (ratingGte.HasValue)
        {
            queryBuilder.Append(" AND rating >= @RatingGte");
            parameters.Add("RatingGte", ratingGte.Value);
        }
        
        if (ratingLte.HasValue)
        {
            queryBuilder.Append(" AND rating <= @RatingLte");
            parameters.Add("RatingLte", ratingLte.Value);
        }

        if (userId.HasValue)
        {
            queryBuilder.Append(" AND userid = @UserId");
            parameters.Add("UserId", userId.Value);
        }
    }
}