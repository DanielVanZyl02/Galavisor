namespace GalavisorApi.Models;

public class UserModel
{
    public int UserId { get; set; }
    public required string Name { get; set; }
    public required string PlanetName { get; set; }
    public required string RoleName { get; set; }
    public bool IsActive { get; set; }
}
