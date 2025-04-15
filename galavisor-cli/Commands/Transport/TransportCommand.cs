using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;

using GalavisorCli.Constants;
using GalavisorCli.Utils;
using GalavisorCli.Models;

namespace GalavisorCli.Commands.Transport;

internal sealed class TransportCommand : AsyncCommand<TransportCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<name>")]
        [Description("The name of the transport")]
        public string Name { get; set; } = null!;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var requestBody = new
            {
                Name = settings.Name
            };

            var url = $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/transport";
            var response = await HttpUtils.Post(url, requestBody);
            
            var transportModel = JsonSerializer.Deserialize<TransportModel>(response.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (transportModel != null)
            {
                var table = new Table
                {
                    Border = TableBorder.Rounded,
                    BorderStyle = Style.Parse("green")
                };

                table.AddColumn("Name");
                table.AddRow(transportModel.Name);

                AnsiConsole.MarkupLine("[green]Transport added successfully[/]");
                AnsiConsole.Write(table);
            }

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
            return 1;
        }
    }
}

internal sealed class GetTransportCommand : AsyncCommand<GetTransportCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-p|--planet <PLANET>")]
        [Description("Get transport for a specific planet")]
        public string? PlanetName { get; set; }

        [CommandOption("-a|--all")]
        [Description("Get all transport options")]
        public bool GetAll { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (!settings.GetAll && string.IsNullOrEmpty(settings.PlanetName))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] You must specify either --all or --planet");
            return 1;
        }

        try
        {
            var url = settings.GetAll ? 
                $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/transport" : 
                $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/transport/planet/{HttpUtility.UrlEncode(settings.PlanetName)}";
            
            var response = await HttpUtils.Get(url);
            
            var table = new Table
            {
                Border = TableBorder.Rounded,
                BorderStyle = Style.Parse("green")
            };

            table.AddColumn("Name");

            var transportList = JsonSerializer.Deserialize<List<TransportModel>>(response.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (transportList != null && transportList.Any())
            {
                string title = settings.GetAll ? 
                    "All Transport Options" : 
                    $"Transport for {settings.PlanetName}";
                
                AnsiConsole.MarkupLine($"[green]{title}[/]");
                
                foreach (var transport in transportList)
                {
                    table.AddRow(transport.Name);
                }
                AnsiConsole.Write(table);
            }
            else
            {
                string message = settings.GetAll ? 
                    "No transport options found" : 
                    $"No transport found for planet {settings.PlanetName}";
                    
                AnsiConsole.MarkupLine($"[yellow]{message}[/]");
            }

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
            return 1;
        }
    }
}

internal sealed class UpdateTransportCommand : AsyncCommand<UpdateTransportCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<name>")]
        [Description("Current name of the transport")]
        public string CurrentName { get; set; } = string.Empty;

        [CommandArgument(1, "<new-name>")]
        [Description("New name for the transport")]
        public string NewName { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var url = $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/transport/{HttpUtility.UrlEncode(settings.CurrentName)}";
            var response = await HttpUtils.Put(url, settings.NewName);
            
            var message = response.GetProperty("message").GetString();
            AnsiConsole.MarkupLine($"[green]{message}[/]");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
    }
}

internal sealed class DeleteTransportCommand : AsyncCommand<DeleteTransportCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<name>")]
        [Description("Name of the transport to delete")]
        public string Name { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var url = $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/transport/{HttpUtility.UrlEncode(settings.Name)}";
            var response = await HttpUtils.Delete(url);
            
            var message = response.GetProperty("message").GetString();
            AnsiConsole.MarkupLine($"[green]{message}[/]");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
    }
}

internal sealed class LinkTransportCommand : AsyncCommand<LinkTransportCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<transport-name>")]
        [Description("Name of the existing transport to link")]
        public string TransportName { get; set; } = string.Empty;

        [CommandArgument(1, "<planet-name>")]
        [Description("Name of the planet to link the transport to")]
        public string PlanetName { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var requestBody = new
        {
            name = settings.TransportName,
            planetName = settings.PlanetName
        };

        try
        {
            var url = $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/transport/link";
            var response = await HttpUtils.Post(url, requestBody);
            
            // Parse the response
            var transport = response.GetProperty("transport").Deserialize<TransportModel>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var status = response.GetProperty("status");
            var isNewlyLinked = status.GetProperty("isNewlyLinked").GetBoolean();
            
            var table = new Table();
            table.AddColumn("Field");
            table.AddColumn("Value");

            if (transport != null)
            {
                table.AddRow("Transport", transport.Name);
                table.AddRow("Planet", transport.PlanetName);
                
                string title;
                Style panelStyle;
                
                if (isNewlyLinked)
                {
                    title = "Transport Linked to Planet";
                    panelStyle = Style.Parse("green");
                }
                else 
                {
                    title = "Transport Already Linked";
                    table.AddRow("Status", "Transport was already linked to this planet");
                    panelStyle = Style.Parse("yellow");
                }

                AnsiConsole.Write(new Panel(table)
                    .Header(title, Justify.Center)
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(panelStyle));
            }

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
    }
} 