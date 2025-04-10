using Spectre.Console.Cli;
using Spectre.Console;
using GalavisorCli.Utils;

namespace GalavisorCli.App;

public static class Run{
    public static async Task RunCli(CommandApp app){
        var knownCommands = GeneralUtils.GetKnownCommands();

        while (true)
        {
            var input = ReadLine.Read("galavisor-cli> ");
            if (string.IsNullOrWhiteSpace(input)) continue;

            var inputArgs = CliHelper.ShellSplit(input);
            if (inputArgs.Length == 0) continue;

            ReadLine.AddHistory(input);

            if (!knownCommands.Contains(inputArgs[0]))
            {
                var suggestion = CliHelper.SuggestCommand(inputArgs[0], knownCommands);
                AnsiConsole.MarkupLine($"[red]Unknown command:[/] {inputArgs[0]}. Did you mean [green]{suggestion}[/]?");
                continue;
            }

            try
            {
                await app.RunAsync(inputArgs);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
            }
        }
    }
}