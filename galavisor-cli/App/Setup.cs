using Microsoft.Extensions.DependencyInjection;
using GalavisorCli.Services;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Spectre.Console;
using GalavisorCli.Utils;
using GalavisorCli.Constants;
using GalavisorCli.Commands.Users;
using GalavisorCli.Commands.System;
using GalavisorCli.Commands.TodoList;
using GalavisorCli.Commands.Reviews;
using GalavisorCli.Commands.Planets;
using GalavisorCli.Commands.Activities;

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
        services.AddTransient<ConfigCommand>();
        services.AddTransient<ReviewCommand>();
        services.AddTransient<GetReviewCommand>();

        services.AddTransient<UpdateReviewCommand>();
        services.AddTransient<DeleteReviewCommand>();

        services.AddTransient<GetPlanetsCommand>();
        services.AddTransient<GetPlanetCommand>();
        services.AddTransient<GetPlanetWeatherCommand>();
        services.AddTransient<AddPlanetCommand>();
        services.AddTransient<UpdatePlanetCommand>();
        services.AddTransient<ActivityCommand>();
        services.AddTransient<GetActivityCommand>();
        services.AddTransient<UpdateActivityCommand>();
        services.AddTransient<DeleteActivityCommand>();
        services.AddTransient<LinkActivityCommand>();


        var serviceProvider = services.BuildServiceProvider();
        var registrar = new DependencyInjectionRegistrar(services);
        var app = new CommandApp(registrar);

        app.Configure(config =>
        {
            config.AddCommand<LoginCommand>(CommandsConstants.login).WithDescription("login to the system");
            config.AddCommand<LogoutCommand>(CommandsConstants.logout).WithDescription("logout of the system");
            config.AddCommand<ConfigCommand>(CommandsConstants.config).WithDescription("See your config information");
            config.AddCommand<AddCommand>(CommandsConstants.add);
            config.AddCommand<ListCommand>(CommandsConstants.list);
            config.AddCommand<UpdateCommand>(CommandsConstants.update);
            config.AddCommand<DeleteCommand>(CommandsConstants.delete);
            config.AddCommand<ExitCommand>(CommandsConstants.exit).WithDescription("Exit the cli");;
            config.AddCommand<HelpCommand>(CommandsConstants.help).WithDescription("See all commands available in the cli");
            config.AddCommand<ReviewCommand>(CommandsConstants.review);
            config.AddCommand<GetReviewCommand>(CommandsConstants.getreview);

            config.AddCommand<UpdateReviewCommand>(CommandsConstants.updatereview);
            config.AddCommand<DeleteReviewCommand>(CommandsConstants.deletereview);

            config.AddCommand<GetPlanetsCommand>(CommandsConstants.planets);
            config.AddCommand<GetPlanetCommand>(CommandsConstants.getplanet);
            config.AddCommand<GetPlanetWeatherCommand>(CommandsConstants.getweather);
            config.AddCommand<AddPlanetCommand>(CommandsConstants.addplanet);
            config.AddCommand<ActivityCommand>(CommandsConstants.addactivity).WithDescription("Add a new activity to the database");
            config.AddCommand<GetActivityCommand>(CommandsConstants.getactivity).WithDescription("Get activities by planet");
            config.AddCommand<UpdateActivityCommand>(CommandsConstants.updateactivity).WithDescription("Update an activity's name");
            config.AddCommand<DeleteActivityCommand>(CommandsConstants.deleteactivity).WithDescription("Delete an activity");
            config.AddCommand<LinkActivityCommand>(CommandsConstants.linkactivity).WithDescription("Link an existing activity to a planet");
        });

        var knownCommands = GeneralUtils.GetKnownCommands();
        ReadLine.HistoryEnabled = true;
        AnsiConsole.Write(
            new FigletText("Galavisor")
            .Color(Color.Teal));
        AnsiConsole.MarkupLine("[green]Welcome to the Interactive CLI![/] Type 'exit' to quit.\n"); // please change this message

        return app;
    }
}