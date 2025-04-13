using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using GalavisorCli.Constants;
using  GalavisorCli.Utils;
using GalavisorCli.Models;

namespace GalavisorCli.Commands.Planets;
internal sealed class GetPlanetsCommand : AsyncCommand<GetPlanetsCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://localhost:5228/planets");

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var planets = JsonSerializer.Deserialize<List<PlanetModel>>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (planets != null)
            {
                var table = new Table();
                table.AddColumn("Planet ID");
                table.AddColumn("Name");
                foreach (var planet in planets)
                {
                    table.AddRow(planet.PlanetId.ToString(), planet.Name.ToString());
                }
                
                AnsiConsole.Write(new Panel(table)
                    .Header("Planets", Justify.Center)
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("green")));
            }

            return 0;
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
        catch (JsonException ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to parse response:[/] {ex.Message}");
            return 1;
        }
    }
}

internal sealed class GetPlanetCommand : AsyncCommand<GetPlanetCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<PLANET ID>")]
        public int planetId { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {

        try {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://localhost:5228/planets/{settings.planetId}");

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var planet = JsonSerializer.Deserialize<List<PlanetModel>>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true})[0];
            if(planet != null) 
            {
                var table = new Table();
                table.AddColumn("Id");
                table.AddColumn("Name");
                table.AddColumn("Atmosphere");
                table.AddColumn("Temperature (C°)");
                table.AddColumn("Colour");

                table.AddRow(planet.PlanetId.ToString(), planet.Name.ToString(), planet.Atmosphere.ToString(), planet.Temperature.ToString(), planet.Colour.ToString());
                AnsiConsole.Write(new Panel(table)
                    .Header($"Planet {planet.Name.ToString()}", Justify.Center)
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("green")));
            }
            return 0;
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
        catch (JsonException ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to parse response:[/] {ex.Message}");
            return 1;
        }
    }
}

internal sealed class GetPlanetWeatherCommand : AsyncCommand<GetPlanetWeatherCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<PLANET ID>")]
        public int planetId { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {

        try {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync($"http://localhost:5228/planets/weather/{settings.planetId}", new {});

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;
            var content = root
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if(content != "") {
                var table = new Table();
                table.AddColumn("Report");
                table.AddRow(content);
                AnsiConsole.Write(new Panel(table)
                    .Header($"Weather Report", Justify.Center)
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("green")));
            }
            
            return 0;
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
        catch (JsonException ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to parse response:[/] {ex.Message}");
            return 1;
        }
    }
}

internal sealed class AddPlanetCommand : AsyncCommand<AddPlanetCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<PLANET NAME>")]
        public string planetName { get; set; }

        [CommandArgument(1, "<ATMOSPHERE>")]
        public string Atmosphere { get; set; }

        [CommandArgument(2, "<TEMPERATURE>")]
        public int Temperature { get; set; }

        [CommandArgument(3, "<COLOUR>")]
        public string Colour { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var requestBody = new
        {
                Name = settings.planetName,
                Atmosphere = settings.Atmosphere,
                Temperature = settings.Temperature,
                Colour = settings.Colour
        };

        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync("http://localhost:5228/planets/add", requestBody);

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var planet = JsonSerializer.Deserialize<PlanetModel>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (planet != null)
            {
                var table = new Table();
                table.AddColumn("Field");
                table.AddColumn("Value");

                table.AddRow("PlanetID", planet.PlanetId.ToString());
                table.AddRow("Name", planet.Name.ToString());
                table.AddRow("Atmosphere", planet.Atmosphere.ToString());
                table.AddRow("Temperature", planet.Temperature.ToString());
                table.AddRow("Colour", planet.Colour.ToString());

                AnsiConsole.Write(new Panel(table)
                    .Header("Planet Added", Justify.Center)
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("green")));
            }

            return 0;
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
        catch (JsonException ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to parse response:[/] {ex.Message}");
            return 1;
        }
    }
}