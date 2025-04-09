namespace GalavisorApi.Models;

public class ReviewModel
{
    public int ReviewId { get; init; }

    public required int Rating { get; set; }

    public string? Comment { get; set; }
}
