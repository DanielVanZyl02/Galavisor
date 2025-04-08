using Microsoft.Extensions.DependencyInjection;
using GalavisorCli.Commands;
using GalavisorCli.Services;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Register your services
services.AddSingleton<AuthService>();

// Register commands
services.AddTransient<LoginCommand>();
services.AddTransient<LogoutCommand>();
services.AddTransient<AddCommand>();
services.AddTransient<ListCommand>();
services.AddTransient<UpdateCommand>();
services.AddTransient<DeleteCommand>();

// Build the service provider
var serviceProvider = services.BuildServiceProvider();

// Create a type registrar using the service provider
var registrar = new DependencyInjectionRegistrar(services);

// Create the command app with the registrar
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.AddCommand<LoginCommand>("login");
    config.AddCommand<LogoutCommand>("logout");
    config.AddCommand<AddCommand>("add");
    config.AddCommand<ListCommand>("list");
    config.AddCommand<UpdateCommand>("update");
    config.AddCommand<DeleteCommand>("delete");
});

return app.Run(args);
