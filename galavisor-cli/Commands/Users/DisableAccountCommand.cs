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
        } else{
            int TargetId = settings.Id ?? -1;
            var IsSelf = TargetId == -1;

            if (IsSelf)
            {
                if (!AnsiConsole.Confirm("[yellow]Are you sure you want to disable your account?[/]"))
                {
                    AnsiConsole.MarkupLine("[grey]Your account will not be disabled.[/]");
                    return 0;
                } else{
                    // User does want to disable their account
                }
            }
            else
            {
                if (!AnsiConsole.Confirm($"[yellow]Are you sure you want to disable the account with ID: {TargetId}?[/]"))
                {
                    AnsiConsole.MarkupLine("[grey]No changes have been made.[/]");
                    return 0;
                } else{
                    // User does want to disable accoutn with the given id
                }
            }

            var EnteredUsername = AnsiConsole.Ask<string>("Enter your [green]username[/]:");
            var CurrentUsername = ConfigStore.Exists(ConfigKeys.GoogleName) ? ConfigStore.Get(ConfigKeys.GoogleName) : "not-set";

            if (!EnteredUsername.Equals(CurrentUsername))
            {
                AnsiConsole.MarkupLine("[grey]That is not the correct username of the currently logged-in User.[/]");
                return 0;
            } else{
                // The username does match the logged in User
            }

            if (!AnsiConsole.Confirm($"[red]Are you absolutely sure you want to disable {(IsSelf ? "your own" : $"the account with ID: {TargetId}")}?[/]\n" +
                                    "[grey]The account can be re-enabled by logging in again.[/]"))
            {
                AnsiConsole.MarkupLine("[grey]Phew! The account is safe.[/]");
                return 0;
            } else{
                // User is completely certain they want to disable said User id
            }

            var Result = await UserService.UpdateUserActiveStatus(TargetId, false);

            Result.Switch(
                Message =>
                {
                    AnsiConsole.MarkupLine($"[red]Encountered an error: {Message}[/]");
                },
                User =>
                {
                    if (IsSelf)
                    {
                        ConfigStore.Remove(ConfigKeys.JwtToken);
                        AnsiConsole.MarkupLine($"[bold red]Your account '{EnteredUsername}' has been disabled.[/]");
                        AnsiConsole.MarkupLine("[grey]You can re-enable it again by logging in.[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[bold red]{User.Name}'s account has been disabled.[/]");
                        AnsiConsole.MarkupLine("[grey]They can re-enable it again by logging in.[/]");
                    }
                    AnsiConsole.Write(TableBuilderUtils.MakeUsersTable([User]));
                }
            );

            return 0;
        }
    }
}
