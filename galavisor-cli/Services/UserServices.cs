using GalavisorCli.Constants;
using  GalavisorCli.Utils;

namespace GalavisorCli.Services;

public class UserService
{
    public static async Task<string> UpdateUserConfig(string Username, string HomePlanet)
    {
        try
        {
            var jsonResponse = await HttpUtils.Patch(
                $"{ConfigStore.Get(ConfigKeys.ServerUri)}/user",
                new Dictionary<string, string> { 
                    { "Username", Username }, 
                    { "HomePlanet", HomePlanet} 
                });

            if (jsonResponse.TryGetProperty("message", out var message))
            {
                return message.GetString() ?? "Server response could not be extracted";
            }
            else
            {
                return "No response from server";
            }
        }
        catch (Exception error)
        {
            return $"Error encountered in updating your username or home planet : {error.Message}";
        }
    }

    public static async Task<string> UpdateUserActiveStatus(int id, bool active)
    {
        try
        {
            var jsonResponse = await HttpUtils.Patch(
                $"{ConfigStore.Get(ConfigKeys.ServerUri)}/user/{id}",
                new Dictionary<string, bool> { 
                    { "active", active }, 
                });

            if (jsonResponse.TryGetProperty("message", out var message))
            {
                return message.GetString() ?? "Server response could not be extracted";
            }
            else
            {
                return "No response from server";
            }
        }
        catch (Exception error)
        {
            return $"Error encountered in updating active status: {error.Message}";
        }
    }

    public static async Task<string> UpdateUserRole(int id, string role)
    {
        try
        {
            var jsonResponse = await HttpUtils.Patch(
                $"{ConfigStore.Get(ConfigKeys.ServerUri)}/user/{id}",
                new Dictionary<string, string> { 
                    { "role", role }, 
                });

            if (jsonResponse.TryGetProperty("message", out var message))
            {
                return message.GetString() ?? "Server response could not be extracted";
            }
            else
            {
                return "No response from server";
            }
        }
        catch (Exception error)
        {
            return $"Error encountered in updating users role: {error.Message}";
        }
    }
}
