using Microsoft.Extensions.DependencyInjection;
using GalavisorCli.Commands;
using GalavisorCli.Services;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Spectre.Console;
using GalavisorCli.Utils;
using GalavisorCli.Constants;
using GalavisorCli.Commands.Users;
using GalavisorCli.Commands.System;
using GalavisorCli.Commands.TodoList;

namespace GalavisorCli.App;

public static class Setup{
    public static CommandApp SetupCli(){
        var services = new ServiceCollection();
        services.AddSingleton<AuthService>();

        services.AddTransient<LoginCommand>();
        services.AddTransient<LogoutCommand>();
        services.AddTransient<AddCommand>();
        services.AddTransient<ListCommand>();
        services.AddTransient<UpdateCommand>();
        services.AddTransient<DeleteCommand>();
        services.AddTransient<ExitCommand>();
        services.AddTransient<HelpCommand>();

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
        });

        var knownCommands = GeneralUtils.GetKnownCommands();
        ReadLine.HistoryEnabled = true;
        AnsiConsole.MarkupLine("[green]Welcome to the Interactive CLI![/] Type 'exit' to quit.\n"); // please change this message

        return app;
    }
}