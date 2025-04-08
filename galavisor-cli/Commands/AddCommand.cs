using Spectre.Console;
using Spectre.Console.Cli;
using GalavisorCli.Constants;
using GalavisorCli.Services;

namespace GalavisorCli.Commands;

public class AddCommand : Command<AddCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<title>")]
        public string Title { get; set; } = "";
    }

    private readonly TodoService _service;
    public AddCommand(TodoService service) => _service = service;

    public override int Execute(CommandContext context, Settings settings)
    {
        if(!ConfigStore.Exists(ConfigKeys.JwtToken)){
            AnsiConsole.MarkupLine("[red]You are not logged in! Please login to use the system.[/]");
            return 0;
        } 
        var item = _service.Add(settings.Title);
        AnsiConsole.MarkupLine($"[green]Added:[/] {item.Id}: {item.Title}");
        return 0;
    }
}
