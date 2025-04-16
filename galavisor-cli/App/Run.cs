using Spectre.Console.Cli;
using Spectre.Console;
using GalavisorCli.Utils;

namespace GalavisorCli.App;

public static class Run{
    public static async Task RunCli(CommandApp App){
        var KnownCommands = GeneralUtils.GetKnownCommands();

        while (true)
        {
            var Input = ReadLine.Read("galavisor-cli> ");
            if (string.IsNullOrWhiteSpace(Input)) continue;

            var InputArgs = CliHelper.ShellSplit(Input);
            if (InputArgs.Length == 0) continue;

            ReadLine.AddHistory(Input);

            if (!KnownCommands.Contains(InputArgs[0]))
            {
                var Suggestion = CliHelper.SuggestCommand(InputArgs[0], KnownCommands);
                AnsiConsole.MarkupLine($"[red]Unknown command:[/] {InputArgs[0]}. Did you mean [green]{Suggestion}[/]?");
                continue;
            }

            try
            {
                await App.RunAsync(InputArgs);
            }
            catch (Exception Error)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {Error.Message}");
            }
        }
    }
}