using GalavisorCli.Constants;
using GalavisorCli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace GalavisorCli.Commands.Users;

public class DisableAccountCommand : Command<CommandSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] CommandSettings settings)
    {
        if(ConfigStore.Exists(ConfigKeys.JwtToken)){
            AnsiConsole.MarkupLine("[red]Please login to use this command.[/]");
            return 0;
        }

        if (!AnsiConsole.Confirm("[yellow]Are you sure you want to disable your account?[/]"))
        {
            AnsiConsole.MarkupLine("[grey]Your account will not be disabled.[/]");
            return 0;
        }

        var username = AnsiConsole.Ask<string>("Enter your [green]username[/]:");

        var Username = ConfigStore.Exists(ConfigKeys.GoogleName) ? ConfigStore.Get(ConfigKeys.GoogleName) : "not-set";

        if(!username.Equals(Username)){
            AnsiConsole.MarkupLine("[grey]That is not the correct username of the currently logged in user.[/]");
            return 0;
        }

        var reallySure = AnsiConsole.Confirm(
            $"[red]Are you absolutely sure you want to disable your account, {username}?[/]\n" +
            "[grey]You can re-enable your account again within the next 30 days.[/]"
        );

        if (!reallySure)
        {
            AnsiConsole.MarkupLine("[grey]Phew! Your account is safe.[/]");
            return 0;
        }

        // TOD: update active status in database
        ConfigStore.Remove(ConfigKeys.JwtToken);
        AnsiConsole.MarkupLine($"[bold red]Your account '{username}' has been disabled.[/]");
        AnsiConsole.MarkupLine("[grey]You have 30 days to re-enable it again. You can re-enable it by simply logging in again.[/]");
        return 0;
    }
}
