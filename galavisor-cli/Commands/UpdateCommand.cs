using Spectre.Console;
using Spectre.Console.Cli;
using GalavisorCli.Constants;
using GalavisorCli.Services;
using  GalavisorCli.Utils;

namespace GalavisorCli.Commands;

public class UpdateCommand : Command<UpdateCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<id>")]
        public int Id { get; set; }

        [CommandOption("-t|--title <title>")]
        public string? Title { get; set; }

        [CommandOption("--done")]
        public bool MarkDone { get; set; }

        [CommandOption("--undone")]
        public bool MarkUndone { get; set; }
    }

    private readonly TodoService _service;
    public UpdateCommand(TodoService service) => _service = service;

    public override int Execute(CommandContext context, Settings settings)
    {
        if(!ConfigStore.Exists(ConfigKeys.JwtToken)){
            AnsiConsole.MarkupLine("[red]You are not logged in! Please login to use the system.[/]");
            return 0;
        } 
        
        var updated = _service.Update(
            settings.Id,
            settings.Title,
            settings.MarkDone ? true : settings.MarkUndone ? false : null
        );

        if (updated)
            AnsiConsole.MarkupLine("[green]Updated successfully.[/]");
        else
            AnsiConsole.MarkupLine("[red]Todo not found.[/]");

        return 0;
    }
}
