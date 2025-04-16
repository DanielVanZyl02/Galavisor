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
        var Services = new ServiceCollection();
        Services.AddSingleton<AuthService>();

        // Auth
        Services.AddTransient<LoginCommand>();
        Services.AddTransient<LogoutCommand>();

        // Users
        Services.AddTransient<ConfigCommand>();
        Services.AddTransient<DisableAccountCommand>();
        Services.AddTransient<EnableAccountCommand>();
        Services.AddTransient<ToggleRoleCommand>();
        Services.AddTransient<UsersCommand>();

        Services.AddTransient<ExitCommand>();
        Services.AddTransient<HelpCommand>();

        Services.AddTransient<ConfigCommand>();
        Services.AddTransient<ReviewCommand>();
        Services.AddTransient<GetReviewCommand>();

        Services.AddTransient<UpdateReviewCommand>();
        Services.AddTransient<DeleteReviewCommand>();

        Services.AddTransient<GetPlanetsCommand>();
        Services.AddTransient<GetPlanetCommand>();
        Services.AddTransient<GetPlanetWeatherCommand>();
        Services.AddTransient<AddPlanetCommand>();
        Services.AddTransient<UpdatePlanetCommand>();
        Services.AddTransient<AddActivityCommand>();
        Services.AddTransient<GetActivityCommand>();
        Services.AddTransient<UpdateActivityCommand>();
        Services.AddTransient<DeleteActivityCommand>();
        Services.AddTransient<LinkActivityCommand>();
        Services.AddTransient<DeletePlanetCommand>();

        // Transport commands
        Services.AddTransient<AddTransportCommand>();
        Services.AddTransient<GetTransportCommand>();
        Services.AddTransient<UpdateTransportCommand>();
        Services.AddTransient<DeleteTransportCommand>();
        Services.AddTransient<LinkTransportCommand>();
      
        var ServicesProvider = Services.BuildServiceProvider();
        var Registrar = new DependencyInjectionRegistrar(Services);
        var App = new CommandApp(Registrar);

        var HelpCommand = new HelpCommand("Galavisor CLI", "A command-line interface for the Galavisor service");
        
        RegisterCommandsWithHelp(HelpCommand);

        App.Configure(config =>
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
                .WithData(HelpCommand);

            config.AddCommand<ReviewCommand>(CommandsConstants.Review);
            config.AddCommand<GetReviewCommand>(CommandsConstants.GetReview);

            config.AddCommand<UpdateReviewCommand>(CommandsConstants.UpdateReview);
            config.AddCommand<DeleteReviewCommand>(CommandsConstants.DeleteReview);

            config.AddCommand<GetPlanetsCommand>(CommandsConstants.planets);
            config.AddCommand<GetPlanetCommand>(CommandsConstants.getplanet);
            config.AddCommand<GetPlanetWeatherCommand>(CommandsConstants.getweather);
            config.AddCommand<AddPlanetCommand>(CommandsConstants.addplanet);
            config.AddCommand<AddActivityCommand>(CommandsConstants.AddActivity).WithDescription("Add a new activity to the database");
            config.AddCommand<GetActivityCommand>(CommandsConstants.GetActivity).WithDescription("Get activities (use with --all or --planet)");
            config.AddCommand<UpdateActivityCommand>(CommandsConstants.UpdateActivity).WithDescription("Update an activity's name");
            config.AddCommand<DeleteActivityCommand>(CommandsConstants.DeleteActivity).WithDescription("Delete an activity");
            config.AddCommand<LinkActivityCommand>(CommandsConstants.LinkActivity).WithDescription("Link an existing activity to a planet");
            config.AddCommand<UpdatePlanetCommand>(CommandsConstants.updateplanet);
            config.AddCommand<DeletePlanetCommand>(CommandsConstants.deleteplanet);

            // Transport commands
            config.AddCommand<AddTransportCommand>(CommandsConstants.AddTransport).WithDescription("Add a new transport option");
            config.AddCommand<GetTransportCommand>(CommandsConstants.GetTransport).WithDescription("Get transport options (use with --all or --planet)");
            config.AddCommand<UpdateTransportCommand>(CommandsConstants.UpdateTransport).WithDescription("Update a transport's name");
            config.AddCommand<DeleteTransportCommand>(CommandsConstants.DeleteTransport).WithDescription("Delete a transport option");
            config.AddCommand<LinkTransportCommand>(CommandsConstants.LinkTransport).WithDescription("Link an existing transport to a planet");
        });
        
        ReadLine.HistoryEnabled = true;
        AnsiConsole.Write(
            new FigletText("Galavisor")
            .Color(Color.Teal));
        AnsiConsole.MarkupLine("[green]Welcome to Galavisor CLI![/] Type 'help' to see available commands.\n");

        return App;
    }

    private static void RegisterCommandsWithHelp(HelpCommand HelpCommand)
    {
        // Auth
        HelpCommand.RegisterCommand<LoginCommand.Settings>(CommandsConstants.login);
        HelpCommand.RegisterCommand<LogoutCommand.Settings>(CommandsConstants.logout);

        // Review commands
        HelpCommand.RegisterCommand<ReviewCommand.Settings>(CommandsConstants.Review);
        HelpCommand.RegisterCommand<GetReviewCommand.Settings>(CommandsConstants.GetReview);
        HelpCommand.RegisterCommand<UpdateReviewCommand.Settings>(CommandsConstants.UpdateReview);
        HelpCommand.RegisterCommand<DeleteReviewCommand.Settings>(CommandsConstants.DeleteReview);

        // Planet commands
        HelpCommand.RegisterCommand<GetPlanetsCommand.Settings>(CommandsConstants.planets);
        HelpCommand.RegisterCommand<GetPlanetCommand.Settings>(CommandsConstants.getplanet);
        HelpCommand.RegisterCommand<GetPlanetWeatherCommand.Settings>(CommandsConstants.getweather);
        HelpCommand.RegisterCommand<AddPlanetCommand.Settings>(CommandsConstants.addplanet);
        HelpCommand.RegisterCommand<UpdatePlanetCommand.Settings>(CommandsConstants.updateplanet);
        HelpCommand.RegisterCommand<DeletePlanetCommand.Settings>(CommandsConstants.deleteplanet);

        // Activity commands
        HelpCommand.RegisterCommand<AddActivityCommand.Settings>(CommandsConstants.AddActivity);
        HelpCommand.RegisterCommand<GetActivityCommand.Settings>(CommandsConstants.GetActivity);
        HelpCommand.RegisterCommand<UpdateActivityCommand.Settings>(CommandsConstants.UpdateActivity);
        HelpCommand.RegisterCommand<DeleteActivityCommand.Settings>(CommandsConstants.DeleteActivity);
        HelpCommand.RegisterCommand<LinkActivityCommand.Settings>(CommandsConstants.LinkActivity);

        // Users
        HelpCommand.RegisterCommand<ConfigCommand.ConfigSettings>(CommandsConstants.config);
        HelpCommand.RegisterCommand<DisableAccountCommand.DisableAccountSettings>(CommandsConstants.disable);
        HelpCommand.RegisterCommand<EnableAccountCommand.EnableAccountSettings>(CommandsConstants.enable);
        HelpCommand.RegisterCommand<ToggleRoleCommand.ToggleRoleSettings>(CommandsConstants.role);
        HelpCommand.RegisterCommand<UsersCommand.Settings>(CommandsConstants.users);

        // Transport commands
        HelpCommand.RegisterCommand<AddTransportCommand.Settings>(CommandsConstants.AddTransport);
        HelpCommand.RegisterCommand<GetTransportCommand.Settings>(CommandsConstants.GetTransport);
        HelpCommand.RegisterCommand<UpdateTransportCommand.Settings>(CommandsConstants.UpdateTransport);
        HelpCommand.RegisterCommand<DeleteTransportCommand.Settings>(CommandsConstants.DeleteTransport);
        HelpCommand.RegisterCommand<LinkTransportCommand.Settings>(CommandsConstants.LinkTransport);
    }
}