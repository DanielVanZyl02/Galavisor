using Spectre.Console;
using Spectre.Console.Cli;
using GalavisorCli.Constants;
using  GalavisorCli.Utils;
using System.ComponentModel;

namespace GalavisorCli.Commands.Auth;

public class LogoutCommand : Command<LogoutCommand.Settings>
{
    [Description("Attempt to login to the system")]
    public class Settings : CommandSettings
    {
    }
    
    public override int Execute(CommandContext context, Settings settings)
    {
        if(ConfigStore.Exists(ConfigKeys.JwtToken)){
            ConfigStore.Remove(ConfigKeys.JwtToken);
            ConfigStore.Remove(ConfigKeys.GoogleName);
            AnsiConsole.MarkupLine("[green]You have been logged out successfully.[/]");
            return 0;
        } else {
            AnsiConsole.MarkupLine("[blue]You are already logged out.[/]");
            return 0;
        }
    }
}
