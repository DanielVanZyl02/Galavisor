using GalavisorCli.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using GalavisorCli.Utils;
using GalavisorCli.Constants;
using System.ComponentModel;

namespace GalavisorCli.Commands.Users;

public class UsersCommand : AsyncCommand<UsersCommand.Settings>
{
    [Description("See all users in the system")]
    public class Settings : CommandSettings
    {
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (!ConfigStore.Exists(ConfigKeys.JwtToken))
        {
            AnsiConsole.MarkupLine("[red]Please login to use this command.[/]");
            return 0;
        }

        var result = await UserService.GetAllUsers();

        result.Switch(
            message =>
            {
                AnsiConsole.MarkupLine($"[red]Encountered an error: {message}[/]");
            },
            users =>
            {
                if (users == null || users.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No users found.[/]");
                } else{
                    var table = new Table();
                    table.AddColumn("[bold]User ID[/]");
                    table.AddColumn("[bold]Name[/]");
                    table.AddColumn("[bold]Planet Name[/]");
                    table.AddColumn("[bold]Role[/]");
                    table.AddColumn("[bold]Active[/]");

                    foreach (var user in users)
                    {
                        table.AddRow(
                            user.UserId.ToString(),
                            user.Name,
                            user.PlanetName,
                            user.RoleName,
                            user.IsActive ? "[green]Active[/]" : "[red]Inactive[/]"
                        );
                    }

                    AnsiConsole.Write(table);

                }
            }
        );

        return 0;
    }
}
