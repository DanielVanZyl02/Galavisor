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

    public virtual async Task<ReviewReturnModel?> GetById(int id)
    {
        using var connection = _db.CreateConnection();
        var review = await connection.QueryFirstOrDefaultAsync<ReviewReturnModel>(
            @" SELECT r.reviewid AS ReviewId,
                    p.planetid AS PlanetId, 
                    r.userid AS UserId,
                    p.name AS PlanetName, 
                    u.name AS UserName, 
                    rating AS Rating, 
                    comment AS Comment
                FROM review r
                INNER JOIN planet p ON p.planetid = r.planetid
                INNER JOIN ""User"" u ON u.userid = r.userid
                WHERE reviewid = @Id", 
            new { Id = id });
        return review;
    }
    
    public async Task<List<ReviewReturnModel>> GetByPlanetId(int planetId, int? ratingEq = null, int? ratingGte = null, int? ratingLte = null, int? userId = null)
    {
        using var connection = _db.CreateConnection();
        
        var queryBuilder = new StringBuilder(@"
                SELECT r.reviewid AS ReviewId,
                    p.planetid AS PlanetId, 
                    r.userid AS UserId,
                    p.name AS PlanetName, 
                    u.name AS UserName, 
                    rating AS Rating, 
                    comment AS Comment
                FROM review r
                INNER JOIN planet p ON p.planetid = r.planetid
                INNER JOIN ""User"" u ON u.userid = r.userid
                WHERE r.planetId = @PlanetId
            ");
        
        var parameters = new DynamicParameters();
        parameters.Add("PlanetId", planetId);
        
        AddRatingFilters(queryBuilder, parameters, ratingEq, ratingGte, ratingLte, userId);
        
        var reviews = await connection.QueryAsync<ReviewReturnModel>(queryBuilder.ToString(), parameters);
        return reviews.ToList();
    }


    public async Task<List<ReviewReturnModel>> GetAll(int? ratingEq = null, int? ratingGte = null, int? ratingLte = null, int? userId = null)
    {
        using var connection = _db.CreateConnection();
        
        var queryBuilder = new StringBuilder(@"
                SELECT r.reviewid AS ReviewId,
                    p.planetid AS PlanetId, 
                    r.userid AS UserId,
                    p.name AS PlanetName, 
                    u.name AS UserName, 
                    rating AS Rating, 
                    comment AS Comment
                FROM review r
                INNER JOIN planet p ON p.planetid = r.planetid
                INNER JOIN ""User"" u ON u.userid = r.userid
            ");
        
        
        var parameters = new DynamicParameters();

        if (ratingEq.HasValue || ratingGte.HasValue || ratingLte.HasValue || userId.HasValue)
        {
            queryBuilder.Append(" WHERE 1=1");
            AddRatingFilters(queryBuilder, parameters, ratingEq, ratingGte, ratingLte, userId);
        }
        
        var reviews = await connection.QueryAsync<ReviewReturnModel>(queryBuilder.ToString(), parameters);

        return reviews.ToList();
    }

    public async Task<ReviewReturnModel> Update(ReviewModel review)
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
            
            SELECT r.reviewid AS ReviewId,
                p.planetid AS PlanetId, 
                r.userid AS UserId,
                p.name AS PlanetName, 
                u.name AS UserName, 
                rating AS Rating, 
                comment AS Comment
            FROM review r
            INNER JOIN planet p ON p.planetid = r.planetid
            INNER JOIN ""User"" u ON u.userid = r.userid
            WHERE reviewid = @ReviewId;";
        
        var updatedReview = await connection.QuerySingleOrDefaultAsync<ReviewReturnModel>(sql, parameters);
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
            queryBuilder.Append(" AND r.userid = @UserId");
            parameters.Add("UserId", userId.Value);
        }
    }
}