using Spectre.Console;
using Spectre.Console.Cli;
using GalavisorCli.Constants;
using System.ComponentModel;
// using GalavisorCli.Services;

namespace GalavisorCli.Commands;
internal sealed class ReviewCommand : AsyncCommand<ReviewCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("Enter a rating")]
        [CommandArgument(0, "<rating>")]
        public float? rating { get; init; }

        [CommandOption("-c|--comment")]
        public string? comment { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var requestBody = new {
          rating = settings.rating.ToString(),
          comment = settings.comment
        };

        var jsonResponse = await HttpUtils.SyncPostAsync("http://localhost:5228/review", requestBody);
        AnsiConsole.WriteLine(jsonResponse.ToString());
        return 0;
    }
}
// public class ReviewCommand : AsyncCommand
// {
//     // private readonly AuthService _service;
//     // public ReviewCommand(AuthService service) => _service = service;
//     public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
//     {
//         var jsonResponse = await HttpUtils.SyncGetAsync("http://localhost:5228/review");
//         AnsiConsole.WriteLine(settings.name);
//         return 0;
//     }
// }
