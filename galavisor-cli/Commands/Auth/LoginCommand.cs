using Spectre.Console;
using Spectre.Console.Cli;
using GalavisorCli.Constants;
using GalavisorCli.Services;
using  GalavisorCli.Utils;

namespace GalavisorCli.Commands.Auth;

public class LoginCommand() : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
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
