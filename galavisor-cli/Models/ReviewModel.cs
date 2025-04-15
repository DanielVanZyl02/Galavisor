using System.Text.Json.Serialization;

namespace GalavisorCli.Models;

public class ReviewModel
{
    [JsonPropertyName("reviewId")]
    public int ReviewId { get; init; }
    [JsonPropertyName("userName")]
    public string? UserName { get; set; }
    [JsonPropertyName("planetName")]
    public string? PlanetName { get; set; }

    [JsonPropertyName("rating")]
    public int? Rating { get; set; }
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}
