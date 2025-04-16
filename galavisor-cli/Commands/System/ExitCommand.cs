using System.ComponentModel;
using Spectre.Console.Cli;

namespace GalavisorCli.Commands.System;

public class ExitCommand : Command<ExitCommand.Settings>
{
    [Description("Exit the cli")]
    public class Settings : CommandSettings
    {
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        Environment.Exit(0);
        return 0;
    }
}
