using Spectre.Console.Cli;

namespace GalavisorCli.Commands;

public class ExitCommand : Command
{
    public override int Execute(CommandContext context)
    {
        Environment.Exit(0);
        return 0;
    }
}
