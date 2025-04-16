using GalavisorCli.Constants;
using GalavisorCli.Services;
using GalavisorCli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace GalavisorCli.Commands.Users;

public class ConfigCommand : AsyncCommand<ConfigCommand.ConfigSettings>
{
    [Description("See your config information")]
    public class ConfigSettings : CommandSettings
    {
        [CommandOption("--list")]
        [Description("List current config values")]
        public bool List { get; set; }

        [CommandOption("--username <USERNAME>")]
        [Description("Set your username")]
        public string? Username { get; set; }
    }

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] ConfigSettings settings)
    {
        if(!ConfigStore.Exists(ConfigKeys.JwtToken)){
            AnsiConsole.MarkupLine("[red]Please login to use this command.[/]");
            return 0;
        }
        
        var Username = ConfigStore.Exists(ConfigKeys.GoogleName) ? ConfigStore.Get(ConfigKeys.GoogleName) : "not-set";

        if (settings.List)
        {
            AnsiConsole.MarkupLine($"[green]Username:[/] {Username ?? "[not set]"}");
            return 0;
        }

        var updated = false;

        if (!string.IsNullOrEmpty(settings.Username))
        {
            Username = settings.Username;
            updated = true;
        }

        if (!updated)
        {
            updated = false;

            if (AnsiConsole.Confirm("Do you want to update your username?"))
            {
                Username = AnsiConsole.Ask<string>("What is your new username?");
                updated = true;
            }
        }

        if (!updated)return 0;

        var User = await UserService.UpdateUserConfig(Username);

        User.Switch(
            Message => {
                AnsiConsole.MarkupLine($"[red]Encountered some error, please try again later: {Message}.[/]");
            },
            User => {
                ConfigStore.Set(ConfigKeys.GoogleName, User.Name);
                AnsiConsole.MarkupLine($"[green]Config updated for Username to {User.Name}.[/]");

                var table = new Table();
                table.AddColumn("[bold]User ID[/]");
                table.AddColumn("[bold]Name[/]");
                table.AddColumn("[bold]Role[/]");
                table.AddColumn("[bold]Active[/]");

                table.AddRow(
                    User.UserId.ToString(),
                    User.Name,
                    User.RoleName,
                    User.IsActive ? "[green]Active[/]" : "[red]Inactive[/]"
                );

                AnsiConsole.Write(table);
            }
        );

        
        return 0;
    }
}
