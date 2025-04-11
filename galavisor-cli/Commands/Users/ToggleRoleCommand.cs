using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

public class ToggleRoleCommand : Command<CommandSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] CommandSettings settings)
    {
        var userId = AnsiConsole.Ask<string>("Enter the [green]User ID[/] of the account you want to update:");

        var newRole = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What role do you want to assign?")
                .AddChoices("Admin", "Traveler")
        );

        if (!AnsiConsole.Confirm($"Are you sure you want to change the role of [cyan]{userId}[/] to [yellow]{newRole}[/]?"))
        {
            AnsiConsole.MarkupLine("[grey]Operation cancelled.[/]");
            return 0;
        }

        AnsiConsole.MarkupLine($"[green]Success:[/] User [cyan]{userId}[/] is now a(n) [yellow]{newRole}[/].");
        return 0;
    }
}
