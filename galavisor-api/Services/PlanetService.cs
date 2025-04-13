using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using GalavisorApi.Models;
using GalavisorApi.Repositories;

namespace GalavisorApi.Services;

public class PlanetService
{
    private readonly PlanetRepository _planetRepository;
    public PlanetService(PlanetRepository repo)
    {
        _planetRepository = repo;
    }

    public async Task<List<PlanetModel>> GetAllPlanets()
    {
        return await _planetRepository.GetAll();
    }

    public async Task<PlanetModel> GetPlanetById(int id)
    {
        return await _planetRepository.GetById(id);
    }

    public async Task<string> GetPlanetWeatherById(int id)
    {
        var planet = await _planetRepository.GetById(id);
        string name = planet.Name;
        string atmosphere = planet.Atmosphere;
        int temperature = planet.Temperature;
        string colour = planet.Colour;

        string initialPrompt = $"generate me a planet report based on the following information " +
        $"It's name is {name}, it has an atmosphere of {atmosphere}, it has an average temperature of ${temperature}c " +
        $"and its colour is {colour}. " +
        "This is for a sci fi system so i don't want to see any response bu the weather report " +
        "Make do with the information you have and do not ask for further information" +
        "Do not include any Mark up language and it must all be plain text and it must be a simple daily weather report " +
        "The date is actually today's date, do not simply put the words today";

        var payload = new
        {
            messages = new[]
            {
                new {role = "user", content = initialPrompt}
            },
            model = "meta-llama/llama-3.2-1b-instruct:free"
        };
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer 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");

        var response = await httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);

        var message = await response.Content.ReadAsStringAsync();
        return message;

    }

    public async Task<PlanetModel> AddPlanet(PlanetModel planet)
    {
        return await _planetRepository.Add(planet);
    }

    public async Task<bool> DeletePlanet(int planetId)
    {
        return await _planetRepository.Delete(planetId);
    }

    public async Task<bool> UpdatePlanet(PlanetModel planet)
    {
        return await _planetRepository.Update(planet);
    }
}