using GalavisorCli.Constants;
using GalavisorCli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace GalavisorCli.Commands.Users;

public class ConfigSettings : CommandSettings
{
    [CommandOption("--list")]
    [Description("List current config values")]
    public bool List { get; set; }

    [CommandOption("--username <USERNAME>")]
    [Description("Set your username")]
    public string? Username { get; set; }

    [CommandOption("--homeplanet <PLANET>")]
    [Description("Set your home planet")]
    public string? HomePlanet { get; set; }
}

public class ConfigCommand : Command<ConfigSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] ConfigSettings settings)
    {
        if(ConfigStore.Exists(ConfigKeys.JwtToken)){
            AnsiConsole.MarkupLine("[red]Please login to use this command.[/]");
            return 0;
        }
        
        var Username = ConfigStore.Exists(ConfigKeys.GoogleName) ? ConfigStore.Get(ConfigKeys.GoogleName) : "not-set";
        var HomePlanet = ConfigStore.Exists(ConfigKeys.HomePlanet) ? ConfigStore.Get(ConfigKeys.HomePlanet) : "not-set";

        if (settings.List)
        {
            AnsiConsole.MarkupLine($"[green]Username:[/] {Username ?? "[not set]"}");
            AnsiConsole.MarkupLine($"[green]Home Planet:[/] {HomePlanet ?? "[not set]"}");
            return 0;
        }

        var updated = false;

        if (!string.IsNullOrEmpty(settings.Username))
        {
            Username = settings.Username;
            updated = true;
        }

        if (!string.IsNullOrEmpty(settings.HomePlanet))
        {
            HomePlanet = settings.HomePlanet;
            updated = true;
        }

        if (!updated)
        {
            updated = false;
            if (AnsiConsole.Confirm("Do you want to update your home planet?"))
            {
                HomePlanet = AnsiConsole.Ask<string>("What is your new home planet?");
                updated = true;
            }

            if (AnsiConsole.Confirm("Do you want to update your username?"))
            {
                Username = AnsiConsole.Ask<string>("What is your new username?");
                updated = true;
            }
        }

        if (!updated)return 0;

        ConfigStore.Set(ConfigKeys.GoogleName, Username);
        ConfigStore.Set(ConfigKeys.HomePlanet, HomePlanet);
        AnsiConsole.MarkupLine($"[green]Config updated for HomePlanet to {HomePlanet} and for Username to {Username}.[/]");
        return 0;
    }
}
