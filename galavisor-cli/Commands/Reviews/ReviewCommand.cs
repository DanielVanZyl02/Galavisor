using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Net;
using System.Linq;

using GalavisorCli.Constants;
using GalavisorCli.Utils;
using GalavisorCli.Models;

namespace GalavisorCli.Commands.Reviews;

internal sealed class ReviewCommand : AsyncCommand<ReviewCommand.Settings>
{
    [Description("Post a review for a specified planet")]
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<PLANET>")]
        [Description("The name of the planet you want to review")]
        public string planetName { get; set; }

        [CommandArgument(1, "<RATING>")]
        [Description("The rating you want to give the planet (1 - 5)")]
        public int rating { get; set; }

        [CommandArgument(2, "[COMMENT]")]
        [Description("The comment you have about the planet")]
        public string? comment { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {   
        try
        {
            var response = await HttpUtils.Get($"{ConfigStore.Get(ConfigKeys.ServerUri)}/planets/name/{settings.planetName}");

            var planetId = response.TryGetProperty("planet", out var planet) 
                ? planet.Deserialize<PlanetModel>()?.PlanetId ?? -1 
                : -1;
                
            if (planetId == -1)
            {
                AnsiConsole.MarkupLine($"[red]Planet does not exist[/]");
                return 1;
            }

            var requestBody = new
            {
                rating = settings.rating, 
                comment = settings.comment,
                planetId = planetId
            };

            var postResponse = await HttpUtils.Post($"{ConfigStore.Get(ConfigKeys.ServerUri)}/reviews", requestBody);

            if(postResponse.TryGetProperty("review", out var createdReview))
            {
                var review = createdReview.Deserialize<ReviewModel>();
                if (review != null)
                {
                    Display.DisplayReviewDetails(review, "Review Posted");
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
    [Description("Get reviews based on specified criteria")]
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "[ID]")]
        [DefaultValue(-1)]
        [Description("The id of a review you want to get")]
        public int id { get; set; }
        
        [CommandOption("--posted")]
        [Description("Include this flag if you want to see only reviews posted by you")]
        public bool posted { get; set; }

        [CommandOption("--planet <PLANET_NAME>")]
        [Description("Include this flag if you want to see reviews posted for a specific planet")]
        public string planetName { get; set; } = string.Empty;

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
        int planetId = -1;
        
        if (!string.IsNullOrEmpty(settings.planetName))
        {
            try
            {
                var response = await HttpUtils.Get($"{ConfigStore.Get(ConfigKeys.ServerUri)}/planets/name/{settings.planetName}");
                planetId = response.TryGetProperty("planet", out var planet) 
                    ? planet.Deserialize<PlanetModel>()?.PlanetId ?? -1 
                    : -1;
                    
                if (planetId == -1)
                {
                    AnsiConsole.MarkupLine($"[red]Planet does not exist[/]");
                    return 1;
                }
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

        try
        {
            if (settings.id != -1 && planetId != -1)
            {
                AnsiConsole.MarkupLine($"[red]Request failed:[/] Cannot search by planet and id");
                return 1;
            }
            string baseUrl = settings.id != -1 
                ? $"{ConfigStore.Get(ConfigKeys.ServerUri)}/reviews/{settings.id}" 
                : planetId != -1 
                    ? $"{ConfigStore.Get(ConfigKeys.ServerUri)}/reviews/planets/{planetId}" 
                    : $"{ConfigStore.Get(ConfigKeys.ServerUri)}/reviews";

            var queryParams = new Dictionary<string, string>
            {
                { "posted", settings.posted ? "true" : null },
                { "ratingEq", settings.ratingEqual != -1 ? settings.ratingEqual.ToString() : null },
                { "ratingGte", settings.ratingGreaterThanOrEqual != -1 ? settings.ratingGreaterThanOrEqual.ToString() : null },
                { "ratingLte", settings.ratingLessThanOrEqual != -1 ? settings.ratingLessThanOrEqual.ToString() : null }
            };
            
            var queryString = string.Join("&", 
                queryParams.Where(p => p.Value != null)
                          .Select(p => $"{p.Key}={p.Value}"));
            
            var requestUrl = string.IsNullOrEmpty(queryString) 
                ? baseUrl
                : $"{baseUrl}?{queryString}";
            
            var response = await HttpUtils.Get(requestUrl);

            if (settings.id != -1)
            {
                if(response.TryGetProperty("reviews", out var reviews))
                {
                    var review = reviews.Deserialize<ReviewModel>();
                    if (review != null)
                    {
                        Display.DisplaySingleReviewAsTable(review);
                    }
                }
            }
            else
            {
                if(response.TryGetProperty("reviews", out var reviewList))
                {
                    var reviews = reviewList.Deserialize<List<ReviewModel>>();

                    if (reviews != null && reviews.Any())
                    {
                        Display.DisplayReviewList(reviews);
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
    [Description("Edit a review you posted")]
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<REVIEW>")]
        [Description("The id of the review you want to update")]
        public int reviewId { get; set; }
        
        [CommandOption("-r | --rating <rating>")]
        [Description("The new rating")]
        [DefaultValue(-1)]
        public int rating { get; set; }
        
        [CommandOption("-c | --comment <comment>")]
        [Description("The new comment")]
        public string comment { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            if(settings.rating == -1 && string.IsNullOrEmpty(settings.comment))
            {
                AnsiConsole.MarkupLine($"[red]Request failed:[/] A new rating or comment is required");
                return 1;                
            }

            var requestBody = new
            {   
                rating = settings.rating != -1 ? (int?)settings.rating : null,
                comment = !string.IsNullOrEmpty(settings.comment) ? settings.comment : null
            };

            var response = await HttpUtils.Put($"{ConfigStore.Get(ConfigKeys.ServerUri)}/reviews/{settings.reviewId}", requestBody);

            if(response.TryGetProperty("status", out var status) && response.TryGetProperty("message", out var message))
            {
                var responseStatus = status.Deserialize<string>();
                var responseMessage = message.Deserialize<string>();
                var color = responseStatus == "Success" ? "green" : "red";
                AnsiConsole.MarkupLine($"[{color}] {responseMessage} [/]");
            }
            else if(response.TryGetProperty("review", out var updatedReview))
            {
                var review = updatedReview.Deserialize<ReviewModel>();
                if (review != null)
                {
                    Display.DisplayReviewDetails(review, "Review Updated");
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
    [Description("Delete a review you posted")]
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<REVIEW>")]
        [Description("The id of the review you want to delete")]
        public int reviewId { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var response = await HttpUtils.Delete($"{ConfigStore.Get(ConfigKeys.ServerUri)}/reviews/{settings.reviewId}");
            
            if(response.TryGetProperty("status", out var status) && response.TryGetProperty("message", out var message))
            {
                var responseStatus = status.Deserialize<string>();
                var responseMessage = message.Deserialize<string>();
                var color = responseStatus == "Success" ? "green" : "red";
                AnsiConsole.MarkupLine($"[{color}] {responseMessage} [/]");
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

internal static class Display
{
    public static void DisplayReviewDetails(ReviewModel review, string headerText = "Review Details")
    {
        var tableData = new[] 
        {
            ("Review ID", review.ReviewId.ToString()),
            ("Planet", review.PlanetName),
            ("Rating", review.Rating.ToString()),
            ("Comment", review.Comment ?? "(none)")
        };
        
        var table = new Table();
        table.AddColumn("Field");
        table.AddColumn("Value");
        
        tableData.ToList().ForEach(row => table.AddRow(row.Item1, row.Item2));

        AnsiConsole.Write(new Panel(table)
            .Header(headerText, Justify.Center)
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("green")));
    }

    public static void DisplayReviewList(IEnumerable<ReviewModel> reviews)
    {
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

        reviews.ToList().ForEach(review => 
            table.AddRow(
                review.ReviewId.ToString(),
                review.UserName ?? "(no user name)",
                review.PlanetName ?? "(no planet name)",
                review.Rating.ToString(), 
                review.Comment ?? "(no comment)"
            )
        );
        
        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[green]Found {reviews.Count()} review(s) matching your criteria.[/]");
    }

    public static void DisplaySingleReviewAsTable(ReviewModel review)
    {
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

        table.AddRow(
            review.ReviewId.ToString(),
            review.UserName ?? "(no user name)",
            review.PlanetName ?? "(no planet name)",
            review.Rating.ToString(), 
            review.Comment ?? "(no comment)"
        );
        
        AnsiConsole.Write(table);
    }
}