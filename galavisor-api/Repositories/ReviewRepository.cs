using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using System.Data;
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
        
        var reviewId = await connection.ExecuteScalarAsync<int>(sql, review);
        review.ReviewId = reviewId;
        return review;
    }

    public async Task<ReviewModel?> GetById(int id)
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

    public async Task<List<ReviewModel>> GetAll()
    {
        using var connection = _db.CreateConnection();
        var reviews = await connection.QueryAsync<ReviewModel>(
            @"SELECT reviewid AS ReviewId, 
                    planetid AS PlanetId, 
                    userid AS UserId, 
                    rating AS Rating, 
                    comment AS Comment 
              FROM review");
        
        return reviews.ToList();
    }

    public async Task<bool> Update(ReviewModel review)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            UPDATE reviews 
            SET planet_id = @PlanetId, 
                user_id = @UserId, 
                rating = @Rating, 
                comment = @Comment
            WHERE review_id = @ReviewId";
        
        var result = await connection.ExecuteAsync(sql, review);
        return result > 0;
    }

    public async Task<bool> Delete(int id)
    {
        using var connection = _db.CreateConnection();
        var result = await connection.ExecuteAsync(
            "DELETE FROM reviews WHERE reviewid = @Id", new { Id = id });
        return result > 0;
    }
}