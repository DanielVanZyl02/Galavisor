using GalavisorCli.Constants;
using GalavisorCli.Services;
using GalavisorCli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace GalavisorCli.Commands.Users;

public class EnableAccountCommand : AsyncCommand<EnableAccountCommand.EnableAccountSettings>
{
    [Description("Enable other peoples accounts")]
    public class EnableAccountSettings : CommandSettings
    {
        [CommandOption("--id <ID>")]
        [Description("ID of the account to enable")]
        public int? Id { get; set; }
    }

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] EnableAccountSettings settings)
    {
        if (!ConfigStore.Exists(ConfigKeys.JwtToken))
        {
            AnsiConsole.MarkupLine("[red]Please login to use this command.[/]");
            return 0;
        }

        var userId = settings.Id ?? -1;
        if(settings.Id == null || userId == -1){
            userId = AnsiConsole.Ask<int>("Enter the [green]User ID[/] of the account you want to enable:c");
        }

        if (!AnsiConsole.Confirm($"[yellow]Are you sure you want to enable the account with ID: {settings.Id}?[/]"))
        {
            AnsiConsole.MarkupLine("[grey]No changes have been made.[/]");
            return 0;
        }

        var enteredUsername = AnsiConsole.Ask<string>("Enter your [green]username[/]:");
        var currentUsername = ConfigStore.Exists(ConfigKeys.GoogleName) ? ConfigStore.Get(ConfigKeys.GoogleName) : "not-set";

        if (!enteredUsername.Equals(currentUsername))
        {
            AnsiConsole.MarkupLine("[grey]That is not the correct username of the currently logged-in user.[/]");
            return 0;
        }

        var result = await UserService.UpdateUserActiveStatus(userId, true);

        result.Switch(
            message =>
            {
                AnsiConsole.MarkupLine($"[red]Encountered an error: {message}[/]");
            },
            user =>
            {
                AnsiConsole.MarkupLine($"[bold green]{user.Name}'s account has been enabled.[/]");
                AnsiConsole.MarkupLine("[grey]They can now log in and continue using their account.[/]");

                var table = new Table();
                table.AddColumn("[bold]User ID[/]");
                table.AddColumn("[bold]Name[/]");
                table.AddColumn("[bold]Planet Name[/]");
                table.AddColumn("[bold]Role[/]");
                table.AddColumn("[bold]Active[/]");

                table.AddRow(
                    user.UserId.ToString(),
                    user.Name,
                    user.PlanetName,
                    user.RoleName,
                    user.IsActive ? "[green]Active[/]" : "[red]Inactive[/]"
                );

                AnsiConsole.Write(table);
            }
        );

        return 0;
    }
}
