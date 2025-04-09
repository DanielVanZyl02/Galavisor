using Spectre.Console;
using Spectre.Console.Cli;
using GalavisorCli.Constants;
using GalavisorCli.Services;
using  GalavisorCli.Utils;

namespace GalavisorCli.Commands;

public class DeleteCommand : Command<DeleteCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<id>")]
        public int Id { get; set; }
    }

    private readonly TodoService _service;
    public DeleteCommand(TodoService service) => _service = service;

    public override int Execute(CommandContext context, Settings settings)
    {
        if(!ConfigStore.Exists(ConfigKeys.JwtToken)){
            AnsiConsole.MarkupLine("[red]You are not logged in! Please login to use the system.[/]");
            return 0;
        } 
        
        if (_service.Delete(settings.Id))
            AnsiConsole.MarkupLine("[green]Deleted.[/]");
        else
            AnsiConsole.MarkupLine("[red]Todo not found.[/]");
        return 0;
    }
}
