using GalavisorCli.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using GalavisorCli.Utils;
using GalavisorCli.Constants;
using System.ComponentModel;

namespace GalavisorCli.Commands.Users;

public class UsersCommand : AsyncCommand<UsersCommand.Settings>
{
    [Description("See all Users in the system")]
    public class Settings : CommandSettings
    {
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (!ConfigStore.Exists(ConfigKeys.JwtToken))
        {
            AnsiConsole.MarkupLine("[red]Please login to use this command.[/]");
            return 0;
        } else{
            var Result = await UserService.GetAllUsers();

            Result.Switch(
                Message =>
                {
                    AnsiConsole.MarkupLine($"[red]Encountered an error: {Message}[/]");
                },
                Users =>
                {
                    if (Users == null || Users.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[yellow]No Users found.[/]");
                    } else{
                        AnsiConsole.Write(TableBuilderUtils.MakeUsersTable(Users));

                    }
                }
            );

            return 0;
        }

    }
}
