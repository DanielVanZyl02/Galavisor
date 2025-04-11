namespace GalavisorApi.Models;

public class ReviewModel
{
    public int ReviewId { get; set; }

    public int PlanetId { get; set; }

    public int UserId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }
}
