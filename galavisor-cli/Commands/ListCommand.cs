using Spectre.Console;
using Spectre.Console.Cli;
using GalavisorCli.Constants;
using GalavisorCli.Services;

namespace GalavisorCli.Commands;

public class ListCommand : Command
{
    private readonly TodoService _service;
    public ListCommand(TodoService service) => _service = service;

    public override int Execute(CommandContext context)
    {
        if(!ConfigStore.Exists(ConfigKeys.JwtToken)){
            AnsiConsole.MarkupLine("[red]You are not logged in! Please login to use the system.[/]");
            return 0;
        } 
        var items = _service.GetAll();
        if (items.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No items yet.[/]");
            return 0;
        }

        foreach (var item in items)
        {
            var status = item.Done ? "[green](done)[/]" : "[red](pending)[/]";
            AnsiConsole.MarkupLine($"[blue]{item.Id}[/]: {item.Title} {status}");
        }
        return 0;
    }
}
