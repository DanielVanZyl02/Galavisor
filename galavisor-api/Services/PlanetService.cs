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
using GalavisorApi.Constants;
using GalavisorApi.Utils;

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
    public async Task<PlanetModel> GetPlanetByName(string name)
    {
        return await _planetRepository.GetByName(name);
    }

    public async Task<string> GetPlanetWeatherById(int id)
    {
        try {
        var planet = await _planetRepository.GetById(id);
        string name = planet.Name;
        string atmosphere = planet.Atmosphere;
        int temperature = planet.Temperature;
        string colour = planet.Colour;
        string initialPrompt = "Generate a daily weather report for a fictional planet with the following details: " +
            $"Planet name: {name}, Atmosphere: {atmosphere}, Average temperature: {temperature}°C, Surface colour: {colour}. "+
            "It must be 100 words max, and your response must contain solely the weather report, nothing else.";

        var payload = new
        {
            messages = new[]
            {
                new {role = "user", content = initialPrompt}
            },
            model = "meta-llama/llama-3.1-8b-instruct:free"
        };
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigStore.Get(ConfigKeys.AiKey));

        var response = await httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);

        var jsonString = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(jsonString); 
        var message = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
        var responseString = $"Date: {DateTime.Today.ToString("d")}\nPlanet: {name}\n\nWeather Report:\n"+message;
        return responseString;
        }catch (HttpRequestException ex)
        {
            return "Failed to communicate with API";
        }
        catch (JsonException ex)
        {
            
            return "Failed to recieve response from API"
        }
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
