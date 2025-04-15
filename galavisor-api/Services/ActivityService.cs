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

    public async Task<List<ActivityModel>> GetActivitiesByPlanet(string planetName)
    {
        const string sql = @"
            SELECT a.* FROM Activity a
            INNER JOIN PlanetActivity pa ON a.ActivityID = pa.ActivityID
            INNER JOIN Planet p ON pa.PlanetID = p.PlanetID
            WHERE p.Name = @PlanetName";
        
        var activities = await _db.QueryAsync<ActivityModel>(sql, new { PlanetName = planetName });
        return activities.ToList();
    }

    public async Task<(ActivityModel Activity, bool IsNewlyCreated)> AddActivity(ActivityModel activity)
    {
        var (activityId, isNewlyCreated) = await CheckActivityExists(activity.Name);
        
        return (new ActivityModel 
        {
            ActivityId = activityId,
            Name = activity.Name
        }, isNewlyCreated);
    }
    
    public async Task<bool> LinkActivityToPlanet(string activityName, string planetName)
    {
        const string checkActivitySql = @"
            SELECT ActivityId FROM Activity 
            WHERE Name = @Name";
            
        var activityId = await _db.QuerySingleOrDefaultAsync<int?>(checkActivitySql, new { Name = activityName });
        
        if (!activityId.HasValue)
        {
            throw new Exception($"Activity '{activityName}' not found");
        }
        
        const string getPlanetIdSql = @"
            SELECT PlanetID FROM Planet 
            WHERE Name = @PlanetName";
        
        var planetId = await _db.QuerySingleOrDefaultAsync<int?>(getPlanetIdSql, new { PlanetName = planetName });
        
        if (planetId == null)
        {
            throw new Exception($"Planet '{planetName}' not found");
        }
        
        const string checkConnectionSql = @"
            SELECT 1 FROM PlanetActivity 
            WHERE PlanetID = @PlanetId AND ActivityID = @ActivityId";
            
        var connectionExists = await _db.QuerySingleOrDefaultAsync<int?>(
            checkConnectionSql, 
            new { PlanetId = planetId, ActivityId = activityId });
        
        if (connectionExists == null)
        {
            const string connectPlanetSql = @"
                INSERT INTO PlanetActivity (PlanetID, ActivityID)
                VALUES (@PlanetId, @ActivityId)";
            
            await _db.ExecuteAsync(connectPlanetSql, 
                new { PlanetId = planetId, ActivityId = activityId });
            return true;
        }
        
        return false;
    }

    private async Task<(int ActivityId, bool IsNewlyCreated)> CheckActivityExists(string activityName)
    {
        const string checkActivitySql = @"
            SELECT ActivityId FROM Activity 
            WHERE Name = @Name";
            
        var existingActivityId = await _db.QuerySingleOrDefaultAsync<int?>(checkActivitySql, new { Name = activityName });
        
        if (existingActivityId.HasValue)
        {
            return (existingActivityId.Value, false);
        }
        else
        {
            const string insertActivitySql = @"
                INSERT INTO Activity (Name)
                VALUES (@Name)
                RETURNING ActivityID";
            
            var newId = await _db.ExecuteScalarAsync<int>(insertActivitySql, new { Name = activityName });
            return (newId, true);
        }
    }

    public async Task<bool> UpdateActivity(string currentName, string newName)
    {
        const string sql = @"
            UPDATE Activity
            SET Name = @NewName
            WHERE Name = @CurrentName";
        
        var affected = await _db.ExecuteAsync(sql, new { CurrentName = currentName, NewName = newName });
        return affected > 0;
    }

    public async Task<bool> DeleteActivity(string activityName)
    {
        const string sql = @"
            DELETE FROM Activity
            WHERE Name = @ActivityName";
        
        var affected = await _db.ExecuteAsync(sql, new { ActivityName = activityName });
        return affected > 0;
    }

    public async Task<List<ActivityModel>> GetAllActivities()
    {
        const string sql = @"
            SELECT ActivityID, Name, '' as PlanetName 
            FROM Activity
            ORDER BY Name";
        
        var activity = await _db.QueryAsync<ActivityModel>(sql);
        return activity.ToList();
    }
} 