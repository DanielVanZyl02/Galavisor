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

internal sealed class AddTransportCommand : AsyncCommand<AddTransportCommand.Settings>
{
    [Description("Add a transport option to the database")]
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<NAME>")]
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
            
            // Parse the response JSON structure to match the API
            var transport = response.GetProperty("transport");
            var transportName = transport.GetProperty("name").GetString();
            
            var isNewlyCreated = response.GetProperty("status").GetProperty("isNewlyCreated").GetBoolean();
            
            var table = new Table
            {
                Border = TableBorder.Rounded,
                BorderStyle = Style.Parse("green")
            };

            table.AddColumn("Name");
            table.AddRow(transportName);

            var title = isNewlyCreated ? 
                "Transport Added Successfully" : 
                "Transport Already Exists";
                
            var messageColor = isNewlyCreated ? "green" : "yellow";
            var panelStyle = isNewlyCreated ? 
                Style.Parse("green") : 
                Style.Parse("yellow");
            
            // Output the message above the panel
            AnsiConsole.MarkupLine($"[{messageColor}]{title}[/]");

    
            var panel = new Panel(table)
                .Header("Transport Details", Justify.Center)
                .Border(BoxBorder.Rounded)
                .BorderStyle(panelStyle);

            AnsiConsole.Write(panel);

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
    [Description("Get all transport options or get the transport for a specific planet")]
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
    [Description("Update the name of an existing transport option")]
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<CURRENT_NAME>")]
        [Description("Current name of the transport")]
        public string CurrentName { get; set; } = string.Empty;

        [CommandArgument(1, "<NEW_NAME>")]
        [Description("New name for the transport")]
        public string NewName { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var url = $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/transport";
            var requestBody = new
            {
                CurrentName = settings.CurrentName,
                NewName = settings.NewName
            };
            var response = await HttpUtils.Put(url, requestBody);
            
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
    [Description("Delete a transport option from the database")]
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<NAME>")]
        [Description("Name of the transport to delete")]
        public string Name { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var url = $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/transport";
            var response = await HttpUtils.DeleteWithBody(url, settings.Name);
            
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
    [Description("Link a transport option to a planet")]
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<TRANSPORT_NAME>")]
        [Description("Name of the existing transport to link")]
        public string TransportName { get; set; } = string.Empty;

        [CommandArgument(1, "<PLANET_NAME>")]
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
                string messageColor;
                Style panelStyle;
                
                if (isNewlyLinked)
                {
                    title = "Transport Linked to Planet";
                    messageColor = "green";
                    panelStyle = Style.Parse("green");
                }
                else 
                {
                    title = "Transport Already Linked";
                    messageColor = "yellow";
                    panelStyle = Style.Parse("yellow");
                    table.AddRow("Status", "Transport was already linked to this planet");
                }

                // Output the message above the panel
                AnsiConsole.MarkupLine($"[{messageColor}]{title}[/]");

                
                var panel = new Panel(table)
                    .Header("Link Details", Justify.Center)
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(panelStyle);

                AnsiConsole.Write(panel);
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