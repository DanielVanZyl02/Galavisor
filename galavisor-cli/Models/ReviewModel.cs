namespace GalavisorCli.Models;

public class ReviewModel
{
    public required int ReviewId { get; init; }

    public required int Rating { get; set; }

    public string? Comment { get; set; }
}
