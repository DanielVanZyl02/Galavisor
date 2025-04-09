using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using GalavisorCli.Constants;
using  GalavisorCli.Utils;
using GalavisorCli.Models;

namespace GalavisorCli.Commands;
internal sealed class ReviewCommand : AsyncCommand<ReviewCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<RATING>")]
        public int rating { get; set; }

        [CommandArgument(1, "[COMMENT]")]
        public string? comment { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var requestBody = new
        {
            rating = settings.rating, 
            comment = settings.comment
        };

        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync("http://localhost:5228/reviews", requestBody);

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var review = JsonSerializer.Deserialize<ReviewModel>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (review != null)
            {
                var table = new Table();
                table.AddColumn("Field");
                table.AddColumn("Value");

                table.AddRow("Review ID", review.ReviewId.ToString());
                table.AddRow("Rating", review.Rating.ToString());
                table.AddRow("Comment", review.Comment ?? "(none)");

                AnsiConsole.Write(new Panel(table)
                    .Header("Review Posted", Justify.Center)
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("green")));
            }

            return 0;
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
        catch (JsonException ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to parse response:[/] {ex.Message}");
            return 1;
        }
    }
}


internal sealed class GetReviewCommand : AsyncCommand<GetReviewCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-i | --id <id>")]
        public int id { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://localhost:5228/reviews");

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var reviews = JsonSerializer.Deserialize<List<ReviewModel>>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (reviews != null)
                {
                    var table = new Table
                    {
                        Border = TableBorder.Rounded,
                        BorderStyle = Style.Parse("green")
                    };

                    table.AddColumn("Review ID");
                    table.AddColumn("Rating");
                    table.AddColumn("Comment");

                    foreach (var review in reviews)
                    {
                        table.AddRow(review.ReviewId.ToString(), review.Rating.ToString(), review.Comment ?? "(no comment)");
                    }
                    AnsiConsole.Write(table);
                }
            return 0;
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
        catch (JsonException ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to parse response:[/] {ex.Message}");
            return 1;
        }
    }
}