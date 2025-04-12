namespace GalavisorApi.Models;

public class UserModel
{
    public required int UserId { get; set; }
    public required string Name { get; set; }
    public required string PlanetName { get; set; }
    public required string RoleName { get; set; }
    public required bool IsActive { get; set; }
    public required string GoogleSubject { get; set; }
}
