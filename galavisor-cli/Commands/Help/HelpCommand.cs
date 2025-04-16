using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Reflection;

namespace GalavisorCli.Commands.Help;

public class HelpCommand : Command<HelpCommand.HelpSettings>
{
    private readonly Dictionary<string, Func<CommandInfo>> _commandInfoProviders = new();
    private readonly string _commandName;
    private readonly string _commandDescription;

    public class HelpSettings : CommandSettings
    {
        [CommandOption("-v|--verbose")]
        [Description("Show detailed help information")]
        public bool Verbose { get; set; }
        [CommandArgument(0, "[command]")]
        [Description("Specific command to show help for")]
        public required string Command { get; set; }
    }

    public class CommandInfo
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public List<ArgumentInfo> Arguments { get; set; } = new();
        public List<OptionInfo> Options { get; set; } = new();
    }

    public class ArgumentInfo
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool Required { get; set; }
        public required string DefaultValue { get; set; }
    }

    public class OptionInfo
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool Required { get; set; }
        public required string DefaultValue { get; set; }
    }

    public HelpCommand() : this("Galavisor CLI", "A command-line interface for the Galavisor service")
    {
    }

    public HelpCommand(string commandName, string commandDescription = "")
    {
        _commandName = commandName;
        _commandDescription = commandDescription;
    }

    public void RegisterCommand<T>(string commandName, Func<CommandInfo> infoProvider) where T : CommandSettings
    {
        _commandInfoProviders[commandName] = infoProvider;
    }

    public void RegisterCommand<T>(string commandName) where T : CommandSettings
    {
        RegisterCommand<T>(commandName, () => ExtractCommandInfo<T>(commandName));
    }

    private CommandInfo ExtractCommandInfo<T>(string commandName) where T : CommandSettings
    {
        var info = new CommandInfo
        {
            Name = commandName,
            Description = GetTypeDescription<T>()
        };

        var properties = typeof(T).GetProperties();
        foreach (var prop in properties)
        {
            var argAttr = prop.GetCustomAttribute<CommandArgumentAttribute>();
            if (argAttr != null)
            {
                string argumentTemplate = prop.Name;
                
                info.Arguments.Add(new ArgumentInfo
                {
                    Name = argumentTemplate,
                    Description = GetPropertyDescription(prop),
                    Required = !HasDefaultValue(prop),
                    DefaultValue = GetDefaultValue(prop)
                });
            }

            var optAttr = prop.GetCustomAttribute<CommandOptionAttribute>();
            if (optAttr != null)
            {
                string optionTemplate = "";
                var constructorInfo = optAttr.GetType().GetConstructors()
                    .FirstOrDefault(c => c.GetParameters().Length > 0);
                
                if (constructorInfo != null)
                {
                    var usageAttr = prop.GetCustomAttributesData()
                        .FirstOrDefault(d => d.AttributeType == typeof(CommandOptionAttribute));
                    
                    if (usageAttr != null && usageAttr.ConstructorArguments.Count > 0)
                    {
                        optionTemplate = usageAttr.ConstructorArguments[0].Value?.ToString() ?? "";
                    }
                }
                
                info.Options.Add(new OptionInfo
                {
                    Name = optionTemplate,
                    Description = GetPropertyDescription(prop),
                    Required = !HasDefaultValue(prop) && prop.PropertyType != typeof(bool),
                    DefaultValue = GetDefaultValue(prop)
                });
            }
        }

        return info;
    }

    private bool HasDefaultValue(PropertyInfo property)
    {
        return property.GetCustomAttribute<DefaultValueAttribute>() != null;
    }

    private string GetTypeDescription<T>()
    {
        var descAttr = typeof(T).GetCustomAttribute<DescriptionAttribute>();
        return descAttr?.Description ?? string.Empty;
    }

    private string GetPropertyDescription(PropertyInfo property)
    {
        var descAttr = property.GetCustomAttribute<DescriptionAttribute>();
        return descAttr?.Description ?? string.Empty;
    }

    private string GetDefaultValue(PropertyInfo property)
    {
        var defaultAttr = property.GetCustomAttribute<DefaultValueAttribute>();
        if (defaultAttr != null)
        {
            if (defaultAttr.Value == null) return "null";
            return defaultAttr.Value.ToString() ?? "";
        }

        if (property.PropertyType.IsValueType)
        {
            var defaultValue = Activator.CreateInstance(property.PropertyType);
            return defaultValue?.ToString() ?? "null";
        }

        return string.Empty;
    }

    public override int Execute(CommandContext context, HelpSettings settings)
    {
        var customHelpCommand = context.Data as HelpCommand;
        if (customHelpCommand != null && customHelpCommand._commandInfoProviders.Any())
        {
            try
            {
                if (!string.IsNullOrEmpty(settings.Command))
                {
                    customHelpCommand.RenderSpecificCommandHelp(settings.Command);
                    return 0;
                }
                else
                {
                    customHelpCommand.RenderCommandHelp(settings.Verbose);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Error displaying help: {ex.Message}");
                RenderSimpleHelp();
                return 1;
            }
        }

        if (!_commandInfoProviders.Any())
        {
            AnsiConsole.WriteLine("No commands registered for help.");
            return 1;
        }

        try
        {
            RenderCommandHelp(settings.Verbose);
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error displaying help: {ex.Message}");
            RenderSimpleHelp();
            return 1;
        }
    }

    private void RenderCommandHelp(bool verbose)
    {
        AnsiConsole.Write(new Rule($"[bold green]{EscapeMarkup(_commandName)} Help[/]").RuleStyle("green").Centered());
        
        if (!string.IsNullOrEmpty(_commandDescription))
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[yellow]{EscapeMarkup(_commandDescription)}[/]");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Available Commands:[/]");
        AnsiConsole.WriteLine();

        foreach (var provider in _commandInfoProviders)
        {
            var commandInfo = provider.Value();

            AnsiConsole.MarkupLine($"[bold cyan]{provider.Key}[/]");
            
            AnsiConsole.MarkupLine($"  {EscapeMarkup(commandInfo.Description ?? "")}");
            
            if (verbose)
            {
                if (commandInfo.Arguments.Count > 0)
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("  [bold underline]Arguments:[/]");
                    foreach (var arg in commandInfo.Arguments)
                    {
                        AnsiConsole.MarkupLine($"    [bold]{arg.Name}[/]");
                        AnsiConsole.MarkupLine($"      {EscapeMarkup(arg.Description)}");
                        
                        if (arg.Required)
                            AnsiConsole.MarkupLine("      [italic]This is a required argument[/]\n");
                    }
                }
            
                if (commandInfo.Options.Count > 0)
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("  [bold underline]Options:[/]");
                    foreach (var opt in commandInfo.Options)
                    {
                        AnsiConsole.MarkupLine($"    [bold]{opt.Name}[/]");
                        AnsiConsole.MarkupLine($"      {EscapeMarkup(opt.Description)}\n");
                    
                    }
                }
            }
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule().RuleStyle("grey"));
            AnsiConsole.WriteLine();
        }
        
        if (!verbose)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[dim]Use --verbose for detailed command information[/]");
        }
    }

    private void RenderSimpleHelp()
    {
        AnsiConsole.WriteLine($"{_commandName} Help");
        AnsiConsole.WriteLine();
        
        if (!string.IsNullOrEmpty(_commandDescription))
        {
            AnsiConsole.WriteLine(_commandDescription);
            AnsiConsole.WriteLine();
        }
        
        AnsiConsole.WriteLine("Available Commands:");
        
        foreach (var provider in _commandInfoProviders)
        {
            var commandInfo = provider.Value();
            AnsiConsole.WriteLine($"  {provider.Key} - {commandInfo.Description}");
        }
        
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("Use 'help --verbose' for more details.");
    }

    private int RenderSpecificCommandHelp(string commandName)
    {
        if (!_commandInfoProviders.TryGetValue(commandName, out var infoProvider))
        {
            AnsiConsole.MarkupLine($"[red]Command '[bold]{commandName}[/]' not found.[/]");
            AnsiConsole.WriteLine("Available commands:");
            foreach (var cmd in _commandInfoProviders.Keys)
            {
                AnsiConsole.WriteLine($"  {cmd}");
            }
            return 1;
        }
        var commandInfo = infoProvider();
        
        AnsiConsole.Write(new Rule($"[bold green]{EscapeMarkup(commandName)} Help[/]").RuleStyle("green").Centered());
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine($"[bold cyan]{commandName}[/]");
        AnsiConsole.MarkupLine($"  {EscapeMarkup(commandInfo.Description ?? "")}");
        
        if (commandInfo.Arguments.Count > 0)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("  [bold underline]Arguments:[/]");
            foreach (var arg in commandInfo.Arguments)
            {
                AnsiConsole.MarkupLine($"    [bold]{arg.Name}[/]");
                AnsiConsole.MarkupLine($"      {EscapeMarkup(arg.Description)}");
                
                if (arg.Required)
                    AnsiConsole.MarkupLine("      [italic]This is a required argument[/]\n");
            }
        }
        
        if (commandInfo.Options.Count > 0)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("  [bold underline]Options:[/]");
            foreach (var opt in commandInfo.Options)
            {
                AnsiConsole.MarkupLine($"    [bold]{opt.Name}[/]");
                AnsiConsole.MarkupLine($"      {EscapeMarkup(opt.Description)}\n");
            }
        }
        
        return 0;
    }

    private string EscapeMarkup(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;
            
        return text
            .Replace("[", "[[")
            .Replace("]", "]]");
    }
}