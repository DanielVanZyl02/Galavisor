using GalavisorCli.Constants;
using GalavisorCli.Services;
using GalavisorCli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace GalavisorCli.Commands.Users;

public class ToggleRoleCommand : AsyncCommand<ToggleRoleCommand.ToggleRoleSettings>
{
    [Description("Change the role of a user")]
    public class ToggleRoleSettings : CommandSettings
    {
        [CommandOption("--id <ID>")]
        [Description("ID of the account to enable")]
        public int? Id { get; set; }
    }

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] ToggleRoleSettings settings)
    {
        if (!ConfigStore.Exists(ConfigKeys.JwtToken))
        {
            AnsiConsole.MarkupLine("[red]Please login to use this command.[/]");
            return 0;
        }

        var userId = settings.Id ?? -1;
        if(settings.Id == null || userId == -1){
            userId = AnsiConsole.Ask<int>("Enter the [green]User ID[/] of the account you want to update:");
        }
        
        var newRole = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What role do you want to assign?")
                .AddChoices("Admin", "Traveler")
        );

        if (!AnsiConsole.Confirm($"Are you sure you want to change the role of [cyan]{userId}[/] to [yellow]{newRole}[/]?"))
        {
            AnsiConsole.MarkupLine("[grey]Operation cancelled.[/]");
            return 0;
        }

        var result = await UserService.UpdateUserRole(userId, newRole);

        result.Switch(
            message =>
            {
                AnsiConsole.MarkupLine($"[red]Encountered an error: {message}[/]");
            },
            user =>
            {
                AnsiConsole.MarkupLine($"[green]Success:[/] User [cyan]{user.UserId}[/] is now a(n) [yellow]{user.RoleName}[/].");

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
