using GalavisorCli.Constants;
using GalavisorCli.Services;
using GalavisorCli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace GalavisorCli.Commands.Users;

public class DisableAccountCommand : AsyncCommand<DisableAccountCommand.DisableAccountSettings>
{
    [Description("Disable your or other peoples accounts")]
    public class DisableAccountSettings : CommandSettings
    {
        [CommandOption("--id <ID>")]
        [Description("ID of the account to disable (optional)")]
        public int? Id { get; set; }
    }

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] DisableAccountSettings settings)
    {
        if (!ConfigStore.Exists(ConfigKeys.JwtToken))
        {
            AnsiConsole.MarkupLine("[red]Please login to use this command.[/]");
            return 0;
        }

        int targetId = settings.Id ?? -1;
        var isSelf = targetId == -1;

        if (isSelf)
        {
            if (!AnsiConsole.Confirm("[yellow]Are you sure you want to disable your account?[/]"))
            {
                AnsiConsole.MarkupLine("[grey]Your account will not be disabled.[/]");
                return 0;
            }
        }
        else
        {
            if (!AnsiConsole.Confirm($"[yellow]Are you sure you want to disable the account with ID: {targetId}?[/]"))
            {
                AnsiConsole.MarkupLine("[grey]No changes have been made.[/]");
                return 0;
            }
        }

        var enteredUsername = AnsiConsole.Ask<string>("Enter your [green]username[/]:");
        var currentUsername = ConfigStore.Exists(ConfigKeys.GoogleName) ? ConfigStore.Get(ConfigKeys.GoogleName) : "not-set";

        if (!enteredUsername.Equals(currentUsername))
        {
            AnsiConsole.MarkupLine("[grey]That is not the correct username of the currently logged-in user.[/]");
            return 0;
        }

        if (!AnsiConsole.Confirm($"[red]Are you absolutely sure you want to disable {(isSelf ? "your own" : $"the account with ID: {targetId}")}?[/]\n" +
                                 "[grey]The account can be re-enabled within the next 30 days by logging in again.[/]"))
        {
            AnsiConsole.MarkupLine("[grey]Phew! The account is safe.[/]");
            return 0;
        }

        var result = await UserService.UpdateUserActiveStatus(targetId, false);

        result.Switch(
            message =>
            {
                AnsiConsole.MarkupLine($"[red]Encountered an error: {message}[/]");
            },
            user =>
            {
                if (isSelf)
                {
                    ConfigStore.Remove(ConfigKeys.JwtToken);
                    AnsiConsole.MarkupLine($"[bold red]Your account '{enteredUsername}' has been disabled.[/]");
                    AnsiConsole.MarkupLine("[grey]You have 30 days to re-enable it again by logging in.[/]");

                    var table = new Table();
                    table.AddColumn("[bold]User ID[/]");
                    table.AddColumn("[bold]Name[/]");
                    table.AddColumn("[bold]Role[/]");
                    table.AddColumn("[bold]Active[/]");

                    table.AddRow(
                        user.UserId.ToString(),
                        user.Name,
                        user.RoleName,
                        user.IsActive ? "[green]Active[/]" : "[red]Inactive[/]"
                    );

                    AnsiConsole.Write(table);
                }
                else
                {
                    AnsiConsole.MarkupLine($"[bold red]{user.Name}'s account has been disabled.[/]");
                    AnsiConsole.MarkupLine("[grey]They can re-enable it again by logging in.[/]");

                    var table = new Table();
                    table.AddColumn("[bold]User ID[/]");
                    table.AddColumn("[bold]Name[/]");
                    table.AddColumn("[bold]Role[/]");
                    table.AddColumn("[bold]Active[/]");

                    table.AddRow(
                        user.UserId.ToString(),
                        user.Name,
                        user.RoleName,
                        user.IsActive ? "[green]Active[/]" : "[red]Inactive[/]"
                    );

                    AnsiConsole.Write(table);
                }
            }
        );

        return 0;
    }
}
