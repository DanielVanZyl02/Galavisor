using Spectre.Console;
using Spectre.Console.Cli;
using GalavisorCli.Constants;
using GalavisorCli.Services;
using  GalavisorCli.Utils;
using System.ComponentModel;

namespace GalavisorCli.Commands.Auth;

public class LoginCommand : AsyncCommand<LoginCommand.Settings>
{
    [Description("Attempt to login to the system")]
    public class Settings : CommandSettings
    {
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if(ConfigStore.Exists(ConfigKeys.JwtToken)){
            AnsiConsole.MarkupLine("[green]You are already logged in.[/]");
            return 0;
        } else {
            AnsiConsole.MarkupLine($"[yellow]Login status:[/] {await AuthService.AttemptGoogleLoginAsync()}");
            return 0;
        }
    }
}
