using Dapper;
using GalavisorApi.Models;
using GalavisorApi.Data;

namespace GalavisorApi.Repositories;

public class UserRepository(DatabaseConnection db)
{
    private readonly DatabaseConnection _db = db;

    public virtual async Task<UserModel?> GetBySub(string GoogleSubject)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserModel>(
            @"SELECT
                u.userid AS UserId,
                u.name AS Name,
                u.isactive AS IsActive,
                r.rolename AS RoleName,
                u.googlesubject AS GoogleSubject
            FROM ""User"" u
            LEFT JOIN userrole r ON u.userroleid = r.userroleid
            WHERE u.googlesubject = @GoogleSubject
            ", 
            new { GoogleSubject });
    }
    
    public async Task<UserModel> CreateUser(string GoogleSubject, string Name)
{
    using var connection = _db.CreateConnection();

    return await connection.QuerySingleAsync<UserModel>(
        @"
            WITH inserted AS (
                INSERT INTO ""User"" (userroleid, planetid, name, isactive, googlesubject)
                VALUES (
                    (SELECT userroleid FROM userrole WHERE rolename = 'Traveler'),
                    1,
                    @Name,
                    true,
                    @GoogleSubject
                )
                ON CONFLICT (googlesubject) DO NOTHING
                RETURNING userid, userroleid, name, isactive, googlesubject
            ),
            fetched AS (
                SELECT * FROM inserted
                UNION
                SELECT userid, userroleid, name, isactive, googlesubject
                FROM ""User""
                WHERE googlesubject = @GoogleSubject
            )
            SELECT
                u.userid AS UserId,
                u.name AS Name,
                u.isactive AS IsActive,
                r.rolename AS RoleName,
                u.googlesubject AS GoogleSubject
            FROM fetched u
            LEFT JOIN userrole r ON u.userroleid = r.userroleid
        ", new {
            GoogleSubject,
            Name
        });
    }

    public virtual async Task<UserModel?> GetById(int UserId)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserModel>(
            @"SELECT
                u.userid AS UserId,
                u.name AS Name,
                u.isactive AS IsActive,
                r.rolename AS RoleName,
                u.googlesubject AS GoogleSubject
            FROM ""User"" u
            LEFT JOIN userrole r ON u.userroleid = r.userroleid
            WHERE u.userid = @UserId", 
            new { UserId });
    }

    public virtual async Task<List<UserModel>> GetAll()
    {
        using var connection = _db.CreateConnection();
        var users = await connection.QueryAsync<UserModel>(
            @"SELECT
                u.userid AS UserId,
                u.name AS Name,
                u.isactive AS IsActive,
                r.rolename AS RoleName,
                u.googlesubject AS GoogleSubject
            FROM ""User"" u
            LEFT JOIN userrole r ON u.userroleid = r.userroleid");
        
        return [.. users];
    }

    public async Task<UserModel> UpdateUserConfig(string GoogleSubject, string NewUserName){
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleAsync<UserModel>(
            @"WITH Updated AS (
                UPDATE ""User""
                SET
                    name = @NewUserName
                WHERE googlesubject = @GoogleSubject
                RETURNING userid, userroleid, name, isactive, googlesubject
            )
            SELECT
                u.userid AS UserId,
                u.name AS Name,
                u.isactive AS IsActive,
                r.rolename AS RoleName,
                u.googlesubject AS GoogleSubject
            FROM Updated u
            LEFT JOIN userrole r ON u.userroleid = r.userroleid"
            ,
            new {
                NewUserName,
                GoogleSubject
            });
    }

    public async Task<UserModel> UpdateActiveStatusBySub(string GoogleSubject, bool IsActive){
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleAsync<UserModel>(
            @"WITH Updated AS (
                UPDATE ""User""
                SET isactive = @IsActive
                WHERE googlesubject = @GoogleSubject
                RETURNING userid, userroleid, name, isactive, googlesubject
            )
            SELECT
                u.userid AS UserId,
                u.name AS Name,
                u.isactive AS IsActive,
                r.rolename AS RoleName,
                u.googlesubject AS GoogleSubject
            FROM Updated u
            LEFT JOIN userrole r ON u.userroleid = r.userroleid
            "
            ,
            new { 
                IsActive,
                GoogleSubject
            });
    }

    public async Task<UserModel> UpdateActiveStatusById(int UserId, bool IsActive){
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleAsync<UserModel>(
            @"WITH Updated AS (
                UPDATE ""User""
                SET isactive = @IsActive
                WHERE userid = @UserId
                RETURNING userid, userroleid, name, isactive, googlesubject
            )
            SELECT
                u.userid AS UserId,
                u.name AS Name,
                u.isactive AS IsActive,
                r.rolename AS RoleName,
                u.googlesubject AS GoogleSubject
            FROM Updated u
            LEFT JOIN userrole r ON u.userroleid = r.userroleid
            "
            ,
            new { 
                IsActive,
                UserId
            });
    }

    public async Task<UserModel> UpdateRole(int UserId, string Role){
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleAsync<UserModel>(
            @"WITH Updated AS (
                UPDATE ""User""
                SET
                userroleid = (SELECT userroleid FROM userrole WHERE rolename = @Role)
                WHERE userid = @UserId
                RETURNING userid, userroleid, name, isactive, googlesubject
            )
            SELECT
                u.userid AS UserId,
                u.name AS Name,
                u.isactive AS IsActive,
                r.rolename AS RoleName,
                u.googlesubject AS GoogleSubject
            FROM Updated u
            LEFT JOIN userrole r ON u.userroleid = r.userroleid
            "
            ,
            new { 
                Role,
                UserId
            });
    }
}