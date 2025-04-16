using System.Text.Json.Serialization;

namespace GalavisorCli.Models;

public class PlanetModel
{
    [JsonPropertyName("planetId")]
    public int PlanetId { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("atmosphere")]
    public required string Atmosphere { get; set; }
    [JsonPropertyName("temperature")]
    public int Temperature { get; set; }
    [JsonPropertyName("colour")]
    public required string Colour { get; set; }
}