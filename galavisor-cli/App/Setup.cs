using Microsoft.Extensions.DependencyInjection;
using GalavisorCli.Services;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Spectre.Console;
using GalavisorCli.Utils;
using GalavisorCli.Constants;
using GalavisorCli.Commands.Users;
using GalavisorCli.Commands.System;
using GalavisorCli.Commands.Reviews;
using GalavisorCli.Commands.Auth;
using GalavisorCli.Commands.Planets;
using GalavisorCli.Commands.Activities;
using GalavisorCli.Commands.Help;
using GalavisorCli.Commands.Transport;


namespace GalavisorCli.App;

public static class Setup
{
    public static CommandApp SetupCli()
    {
        var services = new ServiceCollection();
        services.AddSingleton<AuthService>();

        // Auth
        services.AddTransient<LoginCommand>();
        services.AddTransient<LogoutCommand>();

        // Users
        services.AddTransient<ConfigCommand>();
        services.AddTransient<DisableAccountCommand>();
        services.AddTransient<EnableAccountCommand>();
        services.AddTransient<ToggleRoleCommand>();
        services.AddTransient<UsersCommand>();

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
        services.AddTransient<AddActivityCommand>();
        services.AddTransient<GetActivityCommand>();
        services.AddTransient<UpdateActivityCommand>();
        services.AddTransient<DeleteActivityCommand>();
        services.AddTransient<LinkActivityCommand>();
        services.AddTransient<DeletePlanetCommand>();

        // Transport commands
        services.AddTransient<AddTransportCommand>();
        services.AddTransient<GetTransportCommand>();
        services.AddTransient<UpdateTransportCommand>();
        services.AddTransient<DeleteTransportCommand>();
        services.AddTransient<LinkTransportCommand>();
      
        var serviceProvider = services.BuildServiceProvider();
        var registrar = new DependencyInjectionRegistrar(services);
        var app = new CommandApp(registrar);

        var helpCommand = new HelpCommand("Galavisor CLI", "A command-line interface for the Galavisor service");
        
        RegisterCommandsWithHelp(helpCommand);

        app.Configure(config =>
        {
            // Auth
            config.AddCommand<LoginCommand>(CommandsConstants.login).WithDescription("Attempt to login to the system");
            config.AddCommand<LogoutCommand>(CommandsConstants.logout).WithDescription("Attempt to login to the system");

            // Users
            config.AddCommand<ConfigCommand>(CommandsConstants.config).WithDescription("See your config information");
            config.AddCommand<DisableAccountCommand>(CommandsConstants.disable).WithDescription("Disable your or other peoples accounts");
            config.AddCommand<EnableAccountCommand>(CommandsConstants.enable).WithDescription("Enable other peoples accounts");
            config.AddCommand<ToggleRoleCommand>(CommandsConstants.role).WithDescription("Change the role of a user");
            config.AddCommand<UsersCommand>(CommandsConstants.users).WithDescription("See all users in the system");

            config.AddCommand<ExitCommand>(CommandsConstants.exit).WithDescription("Exit the cli");
            
            config.AddCommand<HelpCommand>("help")
                .WithDescription("Shows help information for available commands")
                .WithData(helpCommand);

            config.AddCommand<ReviewCommand>(CommandsConstants.review);
            config.AddCommand<GetReviewCommand>(CommandsConstants.getreview);

            config.AddCommand<UpdateReviewCommand>(CommandsConstants.updatereview);
            config.AddCommand<DeleteReviewCommand>(CommandsConstants.deletereview);

            config.AddCommand<GetPlanetsCommand>(CommandsConstants.planets);
            config.AddCommand<GetPlanetCommand>(CommandsConstants.getplanet);
            config.AddCommand<GetPlanetWeatherCommand>(CommandsConstants.getweather);
            config.AddCommand<AddPlanetCommand>(CommandsConstants.addplanet);
            config.AddCommand<AddActivityCommand>(CommandsConstants.addactivity).WithDescription("Add a new activity to the database");
            config.AddCommand<GetActivityCommand>(CommandsConstants.getactivity).WithDescription("Get activities (use with --all or --planet)");
            config.AddCommand<UpdateActivityCommand>(CommandsConstants.updateactivity).WithDescription("Update an activity's name");
            config.AddCommand<DeleteActivityCommand>(CommandsConstants.deleteactivity).WithDescription("Delete an activity");
            config.AddCommand<LinkActivityCommand>(CommandsConstants.linkactivity).WithDescription("Link an existing activity to a planet");
            config.AddCommand<UpdatePlanetCommand>(CommandsConstants.updateplanet);
            config.AddCommand<DeletePlanetCommand>(CommandsConstants.deleteplanet);

            // Transport commands
            config.AddCommand<AddTransportCommand>(CommandsConstants.addtransport).WithDescription("Add a new transport option");
            config.AddCommand<GetTransportCommand>(CommandsConstants.gettransport).WithDescription("Get transport options (use with --all or --planet)");
            config.AddCommand<UpdateTransportCommand>(CommandsConstants.updatetransport).WithDescription("Update a transport's name");
            config.AddCommand<DeleteTransportCommand>(CommandsConstants.deletetransport).WithDescription("Delete a transport option");
            config.AddCommand<LinkTransportCommand>(CommandsConstants.linktransport).WithDescription("Link an existing transport to a planet");
        });

        var knownCommands = GeneralUtils.GetKnownCommands();
        ReadLine.HistoryEnabled = true;
        AnsiConsole.Write(
            new FigletText("Galavisor")
            .Color(Color.Teal));
        AnsiConsole.MarkupLine("[green]Welcome to Galavisor CLI![/] Type 'help' to see available commands.\n");

        return app;
    }

    private static void RegisterCommandsWithHelp(HelpCommand helpCommand)
    {
        // Auth
        helpCommand.RegisterCommand<LoginCommand.Settings>(CommandsConstants.login);
        helpCommand.RegisterCommand<LogoutCommand.Settings>(CommandsConstants.logout);

        // Review commands
        helpCommand.RegisterCommand<ReviewCommand.Settings>("review");
        helpCommand.RegisterCommand<GetReviewCommand.Settings>("get-review");
        helpCommand.RegisterCommand<UpdateReviewCommand.Settings>("edit-review");
        helpCommand.RegisterCommand<DeleteReviewCommand.Settings>("delete-review");

        // Planet commands
        helpCommand.RegisterCommand<GetPlanetsCommand.Settings>(CommandsConstants.planets);
        helpCommand.RegisterCommand<GetPlanetCommand.Settings>(CommandsConstants.getplanet);
        helpCommand.RegisterCommand<GetPlanetWeatherCommand.Settings>(CommandsConstants.getweather);
        helpCommand.RegisterCommand<AddPlanetCommand.Settings>(CommandsConstants.addplanet);
        helpCommand.RegisterCommand<UpdatePlanetCommand.Settings>(CommandsConstants.updateplanet);
        helpCommand.RegisterCommand<DeletePlanetCommand.Settings>(CommandsConstants.deleteplanet);

        // Activity commands
        // helpCommand.RegisterCommand<AddActivityCommand.Settings>("activity add");
        // helpCommand.RegisterCommand<GetActivityCommand.Settings>("activities");
        // helpCommand.RegisterCommand<UpdateActivityCommand.Settings>("activity update");
        // helpCommand.RegisterCommand<DeleteActivityCommand.Settings>("activity delete");
        // helpCommand.RegisterCommand<LinkActivityCommand.Settings>("activity link");

        // Users
        helpCommand.RegisterCommand<ConfigCommand.ConfigSettings>(CommandsConstants.config);
        helpCommand.RegisterCommand<DisableAccountCommand.DisableAccountSettings>(CommandsConstants.disable);
        helpCommand.RegisterCommand<EnableAccountCommand.EnableAccountSettings>(CommandsConstants.enable);
        helpCommand.RegisterCommand<ToggleRoleCommand.ToggleRoleSettings>(CommandsConstants.role);
        helpCommand.RegisterCommand<UsersCommand.Settings>(CommandsConstants.users);
    }
}