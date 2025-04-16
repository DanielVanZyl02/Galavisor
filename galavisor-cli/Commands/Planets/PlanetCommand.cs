using Spectre.Console;
using Spectre.Console.Cli;
using System.Text.Json;
using System.Net.Http.Json;

using GalavisorCli.Constants;
using GalavisorCli.Utils;
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
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigStore.Get(ConfigKeys.JwtToken));
            var response = await httpClient.GetAsync($"{ConfigStore.Get(ConfigKeys.ServerUri)}/planets");

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

        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigStore.Get(ConfigKeys.JwtToken));
            var response = await httpClient.GetAsync($"{ConfigStore.Get(ConfigKeys.ServerUri)}/planets/{settings.planetId}");

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var planet = JsonSerializer.Deserialize<PlanetModel>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (planet != null)
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

        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigStore.Get(ConfigKeys.JwtToken));
            var response = await httpClient.PostAsJsonAsync($"{ConfigStore.Get(ConfigKeys.ServerUri)}/planets/weather/{settings.planetId}", new { });

            string content = await response.Content.ReadAsStringAsync();

            if (content != null && content != "")
            {
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
        public required string planetName { get; set; }

        [CommandArgument(1, "<ATMOSPHERE>")]
        public required string Atmosphere { get; set; }

        [CommandArgument(2, "<TEMPERATURE>")]
        public required int Temperature { get; set; }

        [CommandArgument(3, "<COLOUR>")]
        public required string Colour { get; set; }
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
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigStore.Get(ConfigKeys.JwtToken));
            var response = await httpClient.PostAsJsonAsync($"{ConfigStore.Get(ConfigKeys.ServerUri)}/planets/add", requestBody);

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

internal sealed class UpdatePlanetCommand : AsyncCommand<UpdatePlanetCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<PLANET ID>")]
        public int planetId { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {

        try
        {
            using var httpClient = new HttpClient();

            //Getting the new planet to change
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigStore.Get(ConfigKeys.JwtToken));
            var response = await httpClient.GetAsync($"{ConfigStore.Get(ConfigKeys.ServerUri)}/planets/{settings.planetId}");
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var planet = JsonSerializer.Deserialize<PlanetModel>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            response = await httpClient.PatchAsJsonAsync($"{ConfigStore.Get(ConfigKeys.ServerUri)}/planets/update/{settings.planetId}", planet);

            if (AnsiConsole.Confirm($"Current Name: {planet.Name}\nDo you want to update the planet's name?"))
            {
                planet.Name = AnsiConsole.Ask<string>("New name for the planet: ");
            }
            if (AnsiConsole.Confirm($"Current Atmosphere: {planet.Atmosphere}\nDo you want to update the planet's atmosphere?"))
            {
                planet.Atmosphere = AnsiConsole.Ask<string>("New atmosphere for the planet: ");
            }
            if (AnsiConsole.Confirm($"Current Temperature: {planet.Temperature}°C\nDo you want to update the planet's temperature?"))
            {
                planet.Temperature = int.Parse(AnsiConsole.Ask<string>("New temperature for the planet: "));
            }
            if (AnsiConsole.Confirm($"Current Colour: {planet.Colour}\nDo you want to update the planet's colour?"))
            {
                planet.Colour = AnsiConsole.Ask<string>("New colour for the planet: ");
            }

            response = await httpClient.PatchAsJsonAsync($"{ConfigStore.Get(ConfigKeys.ServerUri)}/planets/update/{settings.planetId}", planet);
            response.EnsureSuccessStatusCode();
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

internal sealed class DeletePlanetCommand : AsyncCommand<DeletePlanetCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<PLANET ID>")]
        public int planetId { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {

        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigStore.Get(ConfigKeys.JwtToken));
            var response = await httpClient.GetAsync($"{ConfigStore.Get(ConfigKeys.ServerUri)}/planets/{settings.planetId}");
            (await httpClient.DeleteAsync($"{ConfigStore.Get(ConfigKeys.ServerUri)}/planets/delete/{settings.planetId}")).EnsureSuccessStatusCode();

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var planet = JsonSerializer.Deserialize<PlanetModel>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (planet != null)
            {
                var table = new Table();
                table.AddColumn("Id");
                table.AddColumn("Name");
                table.AddColumn("Atmosphere");
                table.AddColumn("Temperature (C°)");
                table.AddColumn("Colour");

                table.AddRow(planet.PlanetId.ToString(), planet.Name.ToString(), planet.Atmosphere.ToString(), planet.Temperature.ToString(), planet.Colour.ToString());
                AnsiConsole.Write(new Panel(table)
                    .Header($"Deleted {planet.Name.ToString()}", Justify.Center)
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