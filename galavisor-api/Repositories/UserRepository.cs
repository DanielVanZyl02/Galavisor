using Dapper;
using GalavisorApi.Models;
using GalavisorApi.Data;


namespace GalavisorApi.Repositories;

public class UserRepository(DatabaseConnection db)
{
    private readonly DatabaseConnection _db = db;

    public async Task<UserModel?> GetBySub(string sub)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserModel>(
            @"SELECT
                u.UserID AS UserId,
                u.Name AS Name,
                u.IsActive AS IsActive,
                p.Name AS PlanetName,
                r.RoleName AS RoleName
            FROM ""User"" u
            LEFT JOIN Planet p ON u.PlanetID = p.PlanetID
            LEFT JOIN UserRole r ON u.UserRoleID = r.UserRoleID
            WHERE u.GoogleSub = @GoogleSub
            ", 
            new { GoogleSub = sub });
    }

    public async Task<UserModel> CreateUser(string sub, string name)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserModel>(
            @"INSERT INTO ""User"" (UserRoleID, PlanetID, Name, IsActive, GoogleSub)
            VALUES (
                (SELECT UserRoleID FROM UserRole WHERE RoleName = 'Traveler'),
                1,
                @Name,
                true,
                @GoogleSub
            )
            RETURNING
                UserID AS UserId,
                Name,
                IsActive,
                (SELECT Name FROM Planet WHERE PlanetID = 1) AS PlanetName,
                (SELECT RoleName FROM UserRole WHERE UserRoleID = 'User'.UserRoleID) AS RoleName;
            ",
            new { 
                Name = name,
                GoogleSub = sub 
            })
            ?? throw new Exception("Failed to insert user.");
    }

    public async Task<UserModel?> GetById(int UserId)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserModel>(
            @"SELECT
                u.UserID AS UserId,
                u.Name AS Name,
                u.IsActive AS IsActive,
                p.Name AS PlanetName,
                r.RoleName AS RoleName
            FROM ""User"" u
            LEFT JOIN Planet p ON u.PlanetID = p.PlanetID
            LEFT JOIN UserRole r ON u.UserRoleID = r.UserRoleID
            WHERE u.UserID = @UserID", 
            new { UserID = UserId });
    }

    public async Task<List<UserModel>> GetAll()
    {
        using var connection = _db.CreateConnection();
        var users = await connection.QueryAsync<UserModel>(
            @"SELECT
                u.UserID AS UserId,
                u.Name AS Name,
                u.IsActive AS IsActive,
                p.Name AS PlanetName,
                r.RoleName AS RoleName
            FROM ""User"" u
            LEFT JOIN Planet p ON u.PlanetID = p.PlanetID
            LEFT JOIN UserRole r ON u.UserRoleID = r.UserRoleID");
        
        return [.. users];
    }

    public async Task<UserModel> UpdateUserConfig(int UserId, string PlanetName, string NewUserName){
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserModel>(
            @"WITH Updated AS (
                UPDATE ""User""
                SET
                    PlanetID = (SELECT PlanetID FROM Planet WHERE Name = @PlanetName),
                    Name = @NewUserName
                WHERE UserID = @UserID
                RETURNING UserID, UserRoleID, PlanetID, Name, IsActive
            )
            SELECT
                u.UserID AS UserId,
                u.Name,
                u.IsActive,
                p.Name AS PlanetName,
                r.RoleName AS RoleName
            FROM Updated u
            LEFT JOIN Planet p ON u.PlanetID = p.PlanetID
            LEFT JOIN UserRole r ON u.UserRoleID = r.UserRoleID"
            ,
            new {
                PlanetName,
                NewUserName,
                UserID = UserId
            })
            ?? throw new Exception("Failed to update user config.");
    }

    public async Task<UserModel> UpdateActiveStatus(int UserId, bool status){
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserModel>(
            @"WITH Updated AS (
                UPDATE ""User""
                SET IsActive = @IsActive
                WHERE UserID = @UserID
                RETURNING UserID, UserRoleID, PlanetID, Name, IsActive
            )
            SELECT
                u.UserID AS UserId,
                u.Name,
                u.IsActive,
                p.Name AS PlanetName,
                r.RoleName AS RoleName
            FROM Updated u
            LEFT JOIN Planet p ON u.PlanetID = p.PlanetID
            LEFT JOIN UserRole r ON u.UserRoleID = r.UserRoleID
            "
            ,
            new { 
                IsActive = status,
                UserID = UserId
            })
            ?? throw new Exception("Failed to update user config.");
    }

    public async Task<UserModel> UpdateRole(int UserId, string Role){
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserModel>(
            @"WITH Updated AS (
                UPDATE ""User""
                SET
                    UserRoleID = (SELECT UserRoleID FROM UserRole WHERE RoleName = @Role),
                WHERE UserID = @UserID
                RETURNING UserID, UserRoleID, PlanetID, Name, IsActive
            )
            SELECT
                u.UserID AS UserId,
                u.Name,
                u.IsActive,
                p.Name AS PlanetName,
                r.RoleName AS RoleName
            FROM Updated u
            LEFT JOIN Planet p ON u.PlanetID = p.PlanetID
            LEFT JOIN UserRole r ON u.UserRoleID = r.UserRoleID
            "
            ,
            new { 
                Role,
                UserID = UserId
            })
            ?? throw new Exception("Failed to update user config.");
    }
}