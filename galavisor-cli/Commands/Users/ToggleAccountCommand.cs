using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

public class ToggleAccountCommand : Command<CommandSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] CommandSettings settings)
    {
        var userId = AnsiConsole.Ask<string>("Enter the [green]User ID[/] of the account you want to update:");

        AnsiConsole.MarkupLine($"You are about to modify the account for Google Sub: [cyan]{userId}[/]");

        if (!AnsiConsole.Confirm("[yellow]Are you sure you want to continue?[/]"))
        {
            AnsiConsole.MarkupLine("[grey]Operation cancelled.[/]");
            return 0;
        }

        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Do you want to [green]enable[/] or [red]disable[/] the account?")
                .AddChoices("Enable", "Disable")
        );

        if (!AnsiConsole.Confirm($"Are you absolutely sure you want to {action.ToLower()} the account for [cyan]{userId}[/]?"))
        {
            AnsiConsole.MarkupLine("[grey]No changes made.[/]");
            return 0;
        }

        if (action == "Enable")
        {
            // TODO: Actually enable the account in your system
            AnsiConsole.MarkupLine($"[green]The account for {userId} has been enabled.[/]");
        }
        else
        {
            // TODO: Actually disable the account in your system
            AnsiConsole.MarkupLine($"[red]The account for {userId} has been disabled.[/]");
        }

        return 0;
    }
}
