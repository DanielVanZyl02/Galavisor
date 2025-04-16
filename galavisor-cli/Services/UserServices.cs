using System.Text.Json;
using GalavisorApi.Models;
using GalavisorCli.Constants;
using  GalavisorCli.Utils;
using OneOf;

namespace GalavisorCli.Services;

public class UserService
{
    public static async Task<OneOf<string, List<UserModel>>> GetAllUsers()
    {
        try
        {
            var jsonResponse = await HttpUtils.Get($"{ConfigStore.Get(ConfigKeys.ServerUri)}/users");

            if (jsonResponse.TryGetProperty("message", out var message) && jsonResponse.TryGetProperty("error", out var error))
            {
                return $"{message.GetString()}: {error}" ?? "Server response could not be extracted";
            }
            else if(jsonResponse.TryGetProperty("users", out var Users))
            {
                var DeserializedUsers = Users.Deserialize<List<UserModel>>();
                if(DeserializedUsers != null){
                    return DeserializedUsers;
                } else{
                    return "Something went wrong whilst accessing users, please try logging in again";
                }
            }
            else
            {
                return "No response from server";
            }
        }
        catch (Exception error)
        {
            return $"Error encountered in accessing users : {error.Message}";
        }
    }

    public static async Task<OneOf<string, UserModel>> UpdateUserConfig(string Username)
    {
        try
        {
            var jsonResponse = await HttpUtils.Patch(
                $"{ConfigStore.Get(ConfigKeys.ServerUri)}/users",
                new Dictionary<string, string> { 
                    { "Username", Username }
                });

            if (jsonResponse.TryGetProperty("message", out var message) && jsonResponse.TryGetProperty("error", out var error))
            {
                return $"{message.GetString()}: {error}" ?? "Server response could not be extracted";
            }
            else if(jsonResponse.TryGetProperty("user", out var User))
            {
                var DeserializedUser = User.Deserialize<UserModel>();
                if(DeserializedUser != null){
                    return DeserializedUser;
                } else{
                    return "Something went wrong whilst accessing your data, please try logging in again";
                }
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

    public static async Task<OneOf<string, UserModel>> UpdateUserActiveStatus(int id, bool active)
    {
        try
        {
            var jsonResponse = await HttpUtils.Patch(
                $"{ConfigStore.Get(ConfigKeys.ServerUri)}/users/{id}",
                new Dictionary<string, bool> { 
                    { "Active", active }, 
                });

            if (jsonResponse.TryGetProperty("message", out var message) && jsonResponse.TryGetProperty("error", out var error))
            {
                return $"{message.GetString()}: {error}" ?? "Server response could not be extracted";
            }
            else if(jsonResponse.TryGetProperty("user", out var User))
            {
                var DeserializedUser = User.Deserialize<UserModel>();
                if(DeserializedUser != null){
                    return DeserializedUser;
                } else{
                    return "Something went wrong whilst accessing your data, please try logging in again";
                }
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

    public static async Task<OneOf<string, UserModel>> UpdateUserRole(int id, string role)
    {
        try
        {
            var jsonResponse = await HttpUtils.Patch(
                $"{ConfigStore.Get(ConfigKeys.ServerUri)}/users/{id}",
                new Dictionary<string, string> { 
                    { "Role", role }, 
                });

            if (jsonResponse.TryGetProperty("message", out var message) && jsonResponse.TryGetProperty("error", out var error))
            {
                return $"{message.GetString()}: {error}" ?? "Server response could not be extracted";
            }
            else if(jsonResponse.TryGetProperty("user", out var User))
            {
                var DeserializedUser = User.Deserialize<UserModel>();
                if(DeserializedUser != null){
                    return DeserializedUser;
                } else{
                    return "Something went wrong whilst accessing your data, please try logging in again";
                }
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
