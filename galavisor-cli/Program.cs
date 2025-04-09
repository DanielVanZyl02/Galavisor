using Microsoft.Extensions.DependencyInjection;
using GalavisorCli.Commands;
using GalavisorCli.Services;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Spectre.Console;
using GalavisorCli.Utils;
using GalavisorCli.Constants;

// docs: https://spectreconsole.net/

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddSingleton<AuthService>();

        services.AddTransient<LoginCommand>();
        services.AddTransient<LogoutCommand>();
        services.AddTransient<AddCommand>();
        services.AddTransient<ListCommand>();
        services.AddTransient<UpdateCommand>();
        services.AddTransient<DeleteCommand>();
        services.AddTransient<HelpCommand>();
        services.AddTransient<ReviewCommand>();
        services.AddTransient<GetReviewCommand>();


        var serviceProvider = services.BuildServiceProvider();
        var registrar = new DependencyInjectionRegistrar(services);
        var app = new CommandApp(registrar);

        app.Configure(config =>
        {
            config.AddCommand<LoginCommand>(CommandsConstants.login);
            config.AddCommand<LogoutCommand>(CommandsConstants.logout);
            config.AddCommand<AddCommand>(CommandsConstants.add);
            config.AddCommand<ListCommand>(CommandsConstants.list);
            config.AddCommand<UpdateCommand>(CommandsConstants.update);
            config.AddCommand<DeleteCommand>(CommandsConstants.delete);
            config.AddCommand<ExitCommand>(CommandsConstants.exit);
            config.AddCommand<HelpCommand>(CommandsConstants.help);
            config.AddCommand<ReviewCommand>(CommandsConstants.review);
            config.AddCommand<GetReviewCommand>(CommandsConstants.getreview);
        });

        var knownCommands = GeneralUtils.getKnownCommands();
        ReadLine.HistoryEnabled = true;
        AnsiConsole.Write(
            new FigletText("Galavisor")
            .Color(Color.Teal));
        
        AnsiConsole.MarkupLine("[green]Welcome to the Galavisor CLI![/] Type 'exit' to quit.\n");

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