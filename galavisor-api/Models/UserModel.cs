namespace GalavisorApi.Models;

using System.Text.Json.Serialization;

public class UserModel
{
    [JsonPropertyName("userId")]
    public required int UserId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("roleName")]
    public required string RoleName { get; set; }

    [JsonPropertyName("isActive")]
    public required bool IsActive { get; set; }

    [JsonPropertyName("googleSubject")]
    public required string GoogleSubject { get; set; }
}
