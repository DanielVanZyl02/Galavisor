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
        public string Name { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var requestBody = new
        {
            name = settings.Name
        };

        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync("http://localhost:5228/activities", requestBody);

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var activity = JsonSerializer.Deserialize<ActivityModel>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (activity != null)
            {
                var table = new Table();
                table.AddColumn("Field");
                table.AddColumn("Value");

                table.AddRow("Activity ID", activity.ActivityId.ToString());
                table.AddRow("Name", activity.Name);

                AnsiConsole.Write(new Panel(table)
                    .Header("Activity Added", Justify.Center)
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

internal sealed class GetActivityCommand : AsyncCommand<GetActivityCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-p | --planet <planetId>")]
        [DefaultValue(-1)]
        public int PlanetId { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            if (settings.PlanetId == -1)
            {
                AnsiConsole.MarkupLine($"[red]Request failed:[/] Planet ID is required");
                return 1;
            }

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://localhost:5228/activities/planets/{settings.PlanetId}");

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();

            var table = new Table
            {
                Border = TableBorder.Rounded,
                BorderStyle = Style.Parse("green")
            };

            table.AddColumn("Activity ID");
            table.AddColumn("Name");

            var activities = JsonSerializer.Deserialize<List<ActivityModel>>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (activities != null)
            {
                foreach (var activity in activities)
                {
                    table.AddRow(activity.ActivityId.ToString(), activity.Name);
                }
                AnsiConsole.Write(table);
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
        [CommandArgument(0, "<ID>")]
        public int Id { get; set; }

        [CommandArgument(1, "<NAME>")]
        public string Name { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var requestBody = new
        {
            activityId = settings.Id,
            name = settings.Name
        };

        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PutAsJsonAsync($"http://localhost:5228/activities/{settings.Id}", requestBody);

            response.EnsureSuccessStatusCode();
            AnsiConsole.MarkupLine("[green]Activity updated successfully[/]");
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
        [CommandArgument(0, "<ID>")]
        public int Id { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.DeleteAsync($"http://localhost:5228/activities/{settings.Id}");

            response.EnsureSuccessStatusCode();
            AnsiConsole.MarkupLine("[green]Activity deleted successfully[/]");
            return 0;
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
    }
} 