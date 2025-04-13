using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using GalavisorCli.Constants;
using GalavisorCli.Utils;
using GalavisorCli.Models;

namespace GalavisorCli.Commands.Activities;

internal sealed class ActivityCommand : AsyncCommand<ActivityCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<NAME>")]
        [Description("Name of the activity to add")]
        public string Name { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var requestBody = new
        {
            name = settings.Name,
            planetName = string.Empty
        };

        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync("http://localhost:5228/api/activity", requestBody);

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            
            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);
            var activity = responseObj.GetProperty("activity").Deserialize<ActivityModel>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var status = responseObj.GetProperty("status");
            var isNewlyCreated = status.GetProperty("isNewlyCreated").GetBoolean();
            
            var table = new Table();
            table.AddColumn("Field");
            table.AddColumn("Value");

            if (activity != null)
            {
                table.AddRow("Name", activity.Name);
                
                string title;
                Style panelStyle;
                
                if (isNewlyCreated)
                {
                    title = "Activity Added";
                    panelStyle = Style.Parse("green");
                }
                else
                {
                    title = "Activity Already Exists";
                    panelStyle = Style.Parse("yellow");
                }

                AnsiConsole.Write(new Panel(table)
                    .Header(title, Justify.Center)
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(panelStyle));
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

internal sealed class GetActivityCommand : AsyncCommand<GetActivityCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<PLANET>")]
        public string PlanetName { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://localhost:5228/api/activity/planet/{settings.PlanetName}");

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();

            var table = new Table
            {
                Border = TableBorder.Rounded,
                BorderStyle = Style.Parse("green")
            };

            table.AddColumn("Name");

            var activities = JsonSerializer.Deserialize<List<ActivityModel>>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (activities != null && activities.Any())
            {
                foreach (var activity in activities)
                {
                    table.AddRow(activity.Name);
                }
                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.MarkupLine($"[yellow]No activities found for planet {settings.PlanetName}[/]");
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

internal sealed class UpdateActivityCommand : AsyncCommand<UpdateActivityCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<CURRENT_NAME>")]
        public string CurrentName { get; set; } = string.Empty;

        [CommandArgument(1, "<NEW_NAME>")]
        public string NewName { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PutAsJsonAsync($"http://localhost:5228/api/activity/{settings.CurrentName}", settings.NewName);

            response.EnsureSuccessStatusCode();
            AnsiConsole.MarkupLine($"[green]Activity '{settings.CurrentName}' updated to '{settings.NewName}' successfully[/]");
            return 0;
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
    }
}

internal sealed class DeleteActivityCommand : AsyncCommand<DeleteActivityCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<NAME>")]
        [Description("Name of the activity to delete")]
        public string Name { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.DeleteAsync($"http://localhost:5228/api/activity/{settings.Name}");

            response.EnsureSuccessStatusCode();
            AnsiConsole.MarkupLine($"[green]Activity '{settings.Name}' deleted successfully[/]");
            return 0;
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
    }
}

internal sealed class LinkActivityCommand : AsyncCommand<LinkActivityCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<ACTIVITY>")]
        [Description("Name of the existing activity to link")]
        public string ActivityName { get; set; } = string.Empty;

        [CommandArgument(1, "<PLANET>")]
        [Description("Name of the planet to link the activity to")]
        public string PlanetName { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var requestBody = new
        {
            name = settings.ActivityName,
            planetName = settings.PlanetName
        };

        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync("http://localhost:5228/api/activity/link", requestBody);

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            
            // Parse the response
            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);
            var activity = responseObj.GetProperty("activity").Deserialize<ActivityModel>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var status = responseObj.GetProperty("status");
            var isNewlyLinked = status.GetProperty("isNewlyLinked").GetBoolean();
            
            var table = new Table();
            table.AddColumn("Field");
            table.AddColumn("Value");

            if (activity != null)
            {
                table.AddRow("Activity", activity.Name);
                table.AddRow("Planet", activity.PlanetName);
                
                string title;
                Style panelStyle;
                
                if (isNewlyLinked)
                {
                    title = "Activity Linked to Planet";
                    panelStyle = Style.Parse("green");
                }
                else 
                {
                    title = "Activity Already Linked";
                    table.AddRow("Status", "Activity was already linked to this planet");
                    panelStyle = Style.Parse("yellow");
                }

                AnsiConsole.Write(new Panel(table)
                    .Header(title, Justify.Center)
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(panelStyle));
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