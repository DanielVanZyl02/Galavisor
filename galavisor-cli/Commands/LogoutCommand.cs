using Spectre.Console;
using Spectre.Console.Cli;
using GalavisorCli.Constants;

namespace GalavisorCli.Commands;

public class LogoutCommand : Command
{
    public override int Execute(CommandContext context)
    {
        if(ConfigStore.Exists(ConfigKeys.JwtToken)){
            ConfigStore.Remove(ConfigKeys.JwtToken);
            AnsiConsole.MarkupLine("[green]You have been logged out successfully.[/]");
            return 0;
        } else {
            AnsiConsole.MarkupLine("[blue]You are already logged out.[/]");
            return 0;
        }
    }
}
