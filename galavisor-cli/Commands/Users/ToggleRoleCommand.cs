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
    [Description("Change the role of a User")]
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
        } else{
            var UserId = settings.Id ?? -1;
            if(settings.Id == null || UserId == -1){
                UserId = AnsiConsole.Ask<int>("Enter the [green]User ID[/] of the account you want to update:");
            } else{
                // User id is a lready a valid value
            }
            
            var NewRole = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What role do you want to assign?")
                    .AddChoices("Admin", "Traveler")
            );

            if (!AnsiConsole.Confirm($"Are you sure you want to change the role of [cyan]{UserId}[/] to [yellow]{NewRole}[/]?"))
            {
                AnsiConsole.MarkupLine("[grey]Operation cancelled.[/]");
                return 0;
            } else{
                // User does want to change the role of another User
            }

            var Result = await UserService.UpdateUserRole(UserId, NewRole);

            Result.Switch(
                Message =>
                {
                    AnsiConsole.MarkupLine($"[red]Encountered an error: {Message}[/]");
                },
                User =>
                {
                    AnsiConsole.MarkupLine($"[green]Success:[/] User [cyan]{User.UserId}[/] is now a(n) [yellow]{User.RoleName}[/].");
                    AnsiConsole.Write(TableBuilderUtils.MakeUsersTable([User]));
                }
            );

            return 0;
        }

    }
}
