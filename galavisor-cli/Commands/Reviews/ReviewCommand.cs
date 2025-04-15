using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Net;

using GalavisorCli.Constants;
using  GalavisorCli.Utils;
using GalavisorCli.Models;

namespace GalavisorCli.Commands.Reviews;
internal sealed class ReviewCommand : AsyncCommand<ReviewCommand.Settings>
{
    [Description("Post a review for a specified planet")]
    public sealed class Settings : CommandSettings
    {
        

        [CommandArgument(0, "<PLANET>")]
        public int planetId { get; set; }

        [CommandArgument(1, "<RATING>")]
        public int rating { get; set; }

        [CommandArgument(2, "[COMMENT]")]
        public string? comment { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var requestBody = new
        {
            rating = settings.rating, 
            comment = settings.comment,
            planetId = settings.planetId
        };

        try
        {

            var response = await HttpUtils.Post($"{ConfigStore.Get(ConfigKeys.ServerUri)}/reviews", requestBody);

            if(response.TryGetProperty("review", out var createdReview)){
                var review = createdReview.Deserialize<ReviewModel>();
                if (review != null)
                {
                    var table = new Table();
                    table.AddColumn("Field");
                    table.AddColumn("Value");

                    table.AddRow("Review ID", review.ReviewId.ToString());
                    table.AddRow("Planet", review.PlanetName);
                    table.AddRow("Rating", review.Rating.ToString());
                    table.AddRow("Comment", review.Comment ?? "(none)");

                    AnsiConsole.Write(new Panel(table)
                        .Header("Review Posted", Justify.Center)
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(Style.Parse("green")));
                }                
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
        [CommandArgument(0, "[ID]")]
        [DefaultValue(-1)]
        public int id { get; set; }
        [CommandOption("--posted")]
        public bool posted { get; set; }

        [CommandOption("--planet-id <PLANET_ID>")]
        [DefaultValue(-1)]
        public int planetId { get; set; }

        [CommandOption("--rating-eq <RATING>")]
        [Description("Filter reviews with rating equal to this value")]
        [DefaultValue(-1)]
        public int ratingEqual { get; set; }

        [CommandOption("--rating-gte <RATING>")]
        [Description("Filter reviews with rating greater than or equal to this value")]
        [DefaultValue(-1)]
        public int ratingGreaterThanOrEqual { get; set; }

        [CommandOption("--rating-lte <RATING>")]
        [Description("Filter reviews with rating less than or equal to this value")]
        [DefaultValue(-1)]
        public int ratingLessThanOrEqual { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            if (settings.id != -1 && settings.planetId != -1)
            {
                AnsiConsole.MarkupLine($"[red]Request failed:[/] Cannot search by planet and id");
                return 1;
            }

            var baseUrl = $"{ConfigStore.Get(ConfigKeys.ServerUri)}/reviews";
            if (settings.id != -1)
            {
                baseUrl += "/" + settings.id.ToString();
            }
            else if (settings.planetId != -1)
            {
                baseUrl += "/planets/" + settings.planetId.ToString();
            }

            var queryParams = new List<string>();
            
            if(settings.posted){
                queryParams.Add($"posted={settings.posted}");
            }

            if (settings.ratingEqual != -1)
            {
                queryParams.Add($"ratingEq={settings.ratingEqual}");
            }
            
            if (settings.ratingGreaterThanOrEqual != -1)
            {
                queryParams.Add($"ratingGte={settings.ratingGreaterThanOrEqual}");
            }
            
            if (settings.ratingLessThanOrEqual != -1)
            {
                queryParams.Add($"ratingLte={settings.ratingLessThanOrEqual}");
            }
            
            var requestUrl = baseUrl;
            if (queryParams.Count > 0)
            {
                requestUrl += "?" + string.Join("&", queryParams);
            }

            var response = await HttpUtils.Get(requestUrl);

            var table = new Table
            {
                Border = TableBorder.Rounded,
                BorderStyle = Style.Parse("green")
            };

            table.AddColumn("Review ID");
            table.AddColumn("User");
            table.AddColumn("Planet");
            table.AddColumn("Rating");
            table.AddColumn("Comment");

            if (settings.id != -1)
            {
                if(response.TryGetProperty("reviews", out var reviews)){
                    var review = reviews.Deserialize<ReviewModel>();

                    if (review != null)
                    {
                        table.AddRow(
                            review.ReviewId.ToString(),
                            review.UserName ?? "(no user name)",
                            review.PlanetName ?? "(no planet name)",
                            review.Rating.ToString(), 
                            review.Comment ?? "(no comment)"
                        );
                    }
                    AnsiConsole.Write(table);
                }
            }
            else
            {
                if(response.TryGetProperty("reviews", out var reviewList)){
                    var reviews = reviewList.Deserialize<List<ReviewModel>>();

                    if (reviews != null && reviews.Count > 0)
                    {
                        foreach (var review in reviews)
                        {
                            table.AddRow(
                                review.ReviewId.ToString(),
                                review.UserName ?? "(no user name)",
                                review.PlanetName ?? "(no planet name)",
                                review.Rating.ToString(), 
                                review.Comment ?? "(no comment)"
                            );
                        }
                        AnsiConsole.Write(table);
                        AnsiConsole.MarkupLine($"[green]Found {reviews.Count} review(s) matching your criteria.[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[yellow]No reviews found matching the criteria.[/]");
                    }
                }
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


internal sealed class UpdateReviewCommand : AsyncCommand<UpdateReviewCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<REVIEW>")]
        public int reviewId { get; set; }
        [CommandOption("-r | --rating <rating>")]
        [DefaultValue(-1)]
        public int rating { get; set; }
        [CommandOption("-c | --comment <comment>")]
        public string comment { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            if(settings.rating == -1 && string.IsNullOrEmpty(settings.comment)){
                AnsiConsole.MarkupLine($"[red]Request failed:[/] A new rating or comment is required");
                return 1;                
            }

            var requestBody = new
            {   
                rating = settings.rating != -1 ? (int?)settings.rating : null,
                comment = !string.IsNullOrEmpty(settings.comment) ? settings.comment : null
            };

            var response = await HttpUtils.Put($"{ConfigStore.Get(ConfigKeys.ServerUri)}/reviews/{settings.reviewId}", requestBody);

            if(response.TryGetProperty("review", out var updatedReview)){
                var review = updatedReview.Deserialize<ReviewModel>();
                if (review != null)
                {
                    var table = new Table();
                    table.AddColumn("Field");
                    table.AddColumn("Value");

                    table.AddRow("Review ID", review.ReviewId.ToString());
                    table.AddRow("Planet", review.PlanetName);
                    table.AddRow("Rating", review.Rating.ToString());
                    table.AddRow("Comment", review.Comment ?? "(none)");

                    AnsiConsole.Write(new Panel(table)
                        .Header("Review Updated", Justify.Center)
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(Style.Parse("green")));
                }
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

internal sealed class DeleteReviewCommand : AsyncCommand<DeleteReviewCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<REVIEW>")]
        public int reviewId { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var response = await HttpUtils.Delete($"{ConfigStore.Get(ConfigKeys.ServerUri)}/reviews/{settings.reviewId}");
            
            if(response.TryGetProperty("status", out var status) && response.TryGetProperty("message", out var message)){
                var responseStatus = status.Deserialize<string>();
                var responseMessage = message.Deserialize<string>();
                if(responseStatus == "Fail"){
                    AnsiConsole.MarkupLine($"[red] {responseMessage} [/]");
                }else{
                    AnsiConsole.MarkupLine($"[green] {responseMessage} [/]");
                }
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