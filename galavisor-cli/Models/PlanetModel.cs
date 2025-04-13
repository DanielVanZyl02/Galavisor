namespace GalavisorCli.Models;

public class PlanetModel
{
    public int PlanetId { get; set; }
    public required string Name { get; set; }
    public required string Atmosphere { get; set; }
    public int Temperature { get; set; }
    public required string Colour { get; set; }
}