namespace GalavisorCli.Models;

public class ReviewModel
{
    public required int ReviewId { get; init; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }
}
