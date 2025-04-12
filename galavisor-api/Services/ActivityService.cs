using System.Data;
using Dapper;
using GalavisorApi.Models;
using GalavisorApi.Data;

namespace GalavisorApi.Services;

public class ActivityService
{
    private readonly IDbConnection _db;

    public ActivityService(DatabaseConnection db)
    {
        _db = db.CreateConnection();
    }

    public async Task<List<ActivityModel>> GetActivitiesByPlanet(int planetId)
    {
        const string sql = @"
            SELECT a.* FROM Activity a
            INNER JOIN PlanetActivity pa ON a.ActivityID = pa.ActivityID
            WHERE pa.PlanetID = @PlanetId";
        
        var activities = await _db.QueryAsync<ActivityModel>(sql, new { PlanetId = planetId });
        return activities.ToList();
    }

    public async Task<ActivityModel> AddActivity(ActivityModel activity)
    {
        const string sql = @"
            INSERT INTO Activity (Name)
            VALUES (@Name)
            RETURNING ActivityID";
        
        activity.ActivityId = await _db.ExecuteScalarAsync<int>(sql, new { activity.Name });
        return activity;
    }

    public async Task<bool> UpdateActivity(ActivityModel activity)
    {
        const string sql = @"
            UPDATE Activity
            SET Name = @Name
            WHERE ActivityID = @ActivityId";
        
        var affected = await _db.ExecuteAsync(sql, new { activity.Name, activity.ActivityId });
        return affected > 0;
    }

    public async Task<bool> DeleteActivity(int activityId)
    {
        const string sql = @"
            DELETE FROM Activity
            WHERE ActivityID = @ActivityId";
        
        var affected = await _db.ExecuteAsync(sql, new { ActivityId = activityId });
        return affected > 0;
    }
} 