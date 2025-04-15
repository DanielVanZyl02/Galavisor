using System.Data;
using Dapper;
using GalavisorApi.Models;
using GalavisorApi.Data;

namespace GalavisorApi.Services;

public class TransportService{
    private readonly IDbConnection _db;

    public TransportService(DatabaseConnection db)
    {
        _db = db.CreateConnection();
    }

    public async Task<List<TransportModel>> GetTransportByPlanet(string planetName)
    {
        const string sql = @"
            SELECT t.* FROM Transport t
            INNER JOIN PlanetTransport pt ON t.TransportID = pt.TransportID
            INNER JOIN Planet p ON pt.PlanetID = p.PlanetID
            WHERE p.Name = @PlanetName";
        
        var transport = await _db.QueryAsync<TransportModel>(sql, new { PlanetName = planetName });
        return transport.ToList();
    }

    public async Task<(TransportModel Transport, bool IsNewlyCreated)> AddTransport(TransportModel transport)
    {
        var (transportId, isNewlyCreated) = await CheckTransportExists(transport.Name);
        
        return (new TransportModel 
        {
            TransportId = transportId,
            Name = transport.Name
        }, isNewlyCreated);
    }

    private async Task<(int TransportId, bool IsNewlyCreated)> CheckTransportExists(string transportName)
    {
        const string checkTransportSql = @"
            SELECT TransportId FROM Transport 
            WHERE Name = @Name";
            
        var existingTransportId = await _db.QuerySingleOrDefaultAsync<int?>(checkTransportSql, new { Name = transportName });
        
        if (existingTransportId.HasValue)
        {
            return (existingTransportId.Value, false);
        }
        else
        {
            const string insertTransportSql = @"
                INSERT INTO Transport (Name)
                VALUES (@Name)
                RETURNING TransportID";
            
            var newId = await _db.ExecuteScalarAsync<int>(insertTransportSql, new { Name = transportName });
            return (newId, true);
        }
    }

    public async Task<bool> UpdateTransport(string currentName, string newName)
    {
        const string sql = @"
            UPDATE Transport
            SET Name = @NewName
            WHERE Name = @CurrentName";
        
        var affected = await _db.ExecuteAsync(sql, new { CurrentName = currentName, NewName = newName });
        return affected > 0;
    }

    public async Task<bool> DeleteTransport(string transportName)
    {
        const string sql = @"
            DELETE FROM Transport
            WHERE Name = @TransportName";
        
        var affected = await _db.ExecuteAsync(sql, new { TransportName = transportName });
        return affected > 0;
    }

    public async Task<bool> LinkTransportToPlanet(string transportName, string planetName)
    {
        const string checkTransportSql = @"
            SELECT TransportId FROM Transport 
            WHERE Name = @Name";
            
        var transportId = await _db.QuerySingleOrDefaultAsync<int?>(checkTransportSql, new { Name = transportName });
        
        if (!transportId.HasValue)
        {
            throw new Exception($"Transport '{transportName}' not found");
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
            SELECT 1 FROM PlanetTransport 
            WHERE PlanetID = @PlanetId AND TransportID = @TransportId";
            
        var connectionExists = await _db.QuerySingleOrDefaultAsync<int?>(
            checkConnectionSql, 
            new { PlanetId = planetId, TransportId = transportId });
        
        if (connectionExists == null)
        {
            const string connectPlanetSql = @"
                INSERT INTO PlanetTransport (PlanetID, TransportID)
                VALUES (@PlanetId, @TransportId)";
            
            await _db.ExecuteAsync(connectPlanetSql, 
                new { PlanetId = planetId, TransportId = transportId });
            return true;
        }
        
        return false;
    }

    public async Task<List<TransportModel>> GetAllTransport()
    {
        const string sql = @"
            SELECT TransportID, Name, '' as PlanetName 
            FROM Transport
            ORDER BY Name";
        
        var transport = await _db.QueryAsync<TransportModel>(sql);
        return transport.ToList();
    }
}