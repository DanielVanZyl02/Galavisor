using Spectre.Console;
using Spectre.Console.Cli;
using GalavisorCli.Constants;
using GalavisorCli.Services;
using  GalavisorCli.Utils;

namespace GalavisorCli.Commands;

public class LoginCommand : AsyncCommand
{
    private readonly AuthService _service;
    public LoginCommand(AuthService service) => _service = service;

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
