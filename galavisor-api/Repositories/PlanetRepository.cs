using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using System.Data;
using GalavisorApi.Models;
using GalavisorApi.Data;


namespace GalavisorApi.Repositories;

public class PlanetRepository
{
    private readonly DatabaseConnection _db;

    public PlanetRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public async Task<List<PlanetModel>> GetAll()
    {
        using var connection = _db.CreateConnection();
        var planets = await connection.QueryAsync<PlanetModel>(
            @"SELECT *
            FROM planet;");
        return planets.ToList();
    }

    public async Task<List<PlanetModel>> GetById(int planetId)
    {
        using var connection = _db.CreateConnection();
        var planets = await connection.QueryAsync<PlanetModel>(
            @"SELECT *
            FROM planet
            where planetid = @id;",
            new {id = planetId});
        return planets.ToList();
    }

    public async Task<PlanetModel> Add(PlanetModel planet) {
        using var connection = _db.CreateConnection();
        var planetId = await connection.ExecuteScalarAsync<int>(
            @"INSERT INTO public.planet
            (name, atmosphere, temperature, colour)
            VALUES(@name, @atmosphere, @temperature, @colour);"
            , 
            new {
                planet.Name,
                planet.Atmosphere,
                planet.Temperature,
                planet.Colour
            });
            planet.PlanetId = planetId;
        return planet;
    }
}