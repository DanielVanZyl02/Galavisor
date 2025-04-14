using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

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
    }

    public class CommandInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ArgumentInfo> Arguments { get; set; } = new();
        public List<OptionInfo> Options { get; set; } = new();
    }

    public class ArgumentInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public string DefaultValue { get; set; }
    }

    public class OptionInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public string DefaultValue { get; set; }
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
                string argumentTemplate = "";
                
                var valueField = argAttr.GetType().GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
                if (valueField != null)
                {
                    argumentTemplate = valueField.GetValue(argAttr)?.ToString() ?? "";
                }
                
                info.Arguments.Add(new ArgumentInfo
                {
                    Name = argumentTemplate,
                    Description = GetPropertyDescription(prop),
                    Required = argumentTemplate.StartsWith("<") && argumentTemplate.EndsWith(">"),
                    DefaultValue = GetDefaultValue(prop)
                });
            }

            var optAttr = prop.GetCustomAttribute<CommandOptionAttribute>();
            if (optAttr != null)
            {
                string optionTemplate = "";
                
                var templateField = optAttr.GetType().GetField("_template", BindingFlags.NonPublic | BindingFlags.Instance);
                if (templateField != null)
                {
                    optionTemplate = templateField.GetValue(optAttr)?.ToString() ?? "";
                }
                else 
                {
                    var templateProperty = optAttr.GetType().GetProperty("Template", BindingFlags.Public | BindingFlags.Instance);
                    if (templateProperty != null)
                    {
                        optionTemplate = templateProperty.GetValue(optAttr)?.ToString() ?? "";
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
            return defaultAttr.Value.ToString();
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
                customHelpCommand.RenderCommandHelp(settings.Verbose);
                return 0;
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
        AnsiConsole.WriteLine("Available Commands:");
        AnsiConsole.WriteLine();

        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.BorderStyle = Style.Parse("green");
        table.AddColumn("Command");
        table.AddColumn("Description");
        
        if (verbose)
        {
            table.AddColumn("Arguments");
            table.AddColumn("Options");
        }

        foreach (var provider in _commandInfoProviders)
        {
            var commandInfo = provider.Value();
            
            if (verbose)
            {
                var args = FormatArgumentsList(commandInfo.Arguments);
                var opts = FormatOptionsList(commandInfo.Options);
                
                table.AddRow(
                    provider.Key,
                    EscapeMarkup(commandInfo.Description ?? ""),
                    args,
                    opts
                );
            }
            else
            {
                table.AddRow(
                    provider.Key,
                    EscapeMarkup(commandInfo.Description ?? "")
                );
            }
        }

        AnsiConsole.Write(table);
        
        if (!verbose)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("Use --verbose for detailed command information");
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

    private string FormatArgumentsList(List<ArgumentInfo> arguments)
    {
        if (arguments.Count == 0) return "";
        
        var sb = new StringBuilder();
        foreach (var arg in arguments)
        {
            sb.AppendLine($"{arg.Name}:");
            sb.AppendLine($"  {EscapeMarkup(arg.Description)}");
            
            if (arg.Required)
                sb.AppendLine("  Required");
                
            if (!string.IsNullOrEmpty(arg.DefaultValue))
                sb.AppendLine($"  Default: {EscapeMarkup(arg.DefaultValue)}");
                
            sb.AppendLine();
        }
        return sb.ToString().TrimEnd();
    }

    private string FormatOptionsList(List<OptionInfo> options)
    {
        if (options.Count == 0) return "";
        
        var sb = new StringBuilder();
        foreach (var opt in options)
        {
            sb.AppendLine($"{opt.Name}:");
            sb.AppendLine($"  {EscapeMarkup(opt.Description)}");
            
            if (opt.Required)
                sb.AppendLine("  Required");
                
            if (!string.IsNullOrEmpty(opt.DefaultValue))
                sb.AppendLine($"  Default: {EscapeMarkup(opt.DefaultValue)}");
                
            sb.AppendLine();
        }
        return sb.ToString().TrimEnd();
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