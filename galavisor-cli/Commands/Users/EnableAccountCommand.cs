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
        } else{
            var UserId = settings.Id ?? -1;
            if(settings.Id == null || UserId == -1){
                UserId = AnsiConsole.Ask<int>("Enter the [green]User ID[/] of the account you want to enable:c");
            } else{
                // User id is a lready a valid value
            }

            if (!AnsiConsole.Confirm($"[yellow]Are you sure you want to enable the account with ID: {settings.Id}?[/]"))
            {
                AnsiConsole.MarkupLine("[grey]No changes have been made.[/]");
                return 0;
            } else{
                // User does want to enable account with specified id
            }

            var EnteredUsername = AnsiConsole.Ask<string>("Enter your [green]username[/]:");
            var CurrentUsername = ConfigStore.Exists(ConfigKeys.GoogleName) ? ConfigStore.Get(ConfigKeys.GoogleName) : "not-set";

            if (!EnteredUsername.Equals(CurrentUsername))
            {
                AnsiConsole.MarkupLine("[grey]That is not the correct username of the currently logged-in User.[/]");
                return 0;
            } else{
                // The User's username does match correctly
            }

            var Result = await UserService.UpdateUserActiveStatus(UserId, true);

            Result.Switch(
                Message =>
                {
                    AnsiConsole.MarkupLine($"[red]Encountered an error: {Message}[/]");
                },
                User =>
                {
                    AnsiConsole.MarkupLine($"[bold green]{User.Name}'s account has been enabled.[/]");
                    AnsiConsole.MarkupLine("[grey]They can now log in and continue using their account.[/]");
                    AnsiConsole.Write(TableBuilderUtils.MakeUsersTable([User]));
                }
            );

            return 0;
        }

    }
}
