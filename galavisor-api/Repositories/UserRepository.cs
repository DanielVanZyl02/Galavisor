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
                r.RoleName AS RoleName,
                u.GoogleSubject AS GoogleSubject
            FROM ""User"" u
            LEFT JOIN Planet p ON u.PlanetID = p.PlanetID
            LEFT JOIN UserRole r ON u.UserRoleID = r.UserRoleID
            WHERE u.GoogleSubject = @GoogleSubject
            ", 
            new { GoogleSubject = sub });
    }
}