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
            var JsonResponse = await HttpUtils.Get($"{ConfigStore.Get(ConfigKeys.ServerUri)}/users");

            if (JsonResponse.TryGetProperty("message", out var Message) && JsonResponse.TryGetProperty("error", out var Error))
            {
                return $"{Message.GetString()}: {Error}" ?? "Server response could not be extracted";
            }
            else if(JsonResponse.TryGetProperty("users", out var Users))
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
        catch (Exception Error)
        {
            return $"Error encountered in accessing users : {Error.Message}";
        }
    }

    public static async Task<OneOf<string, UserModel>> UpdateUserConfig(string Username)
    {
        try
        {
            var JsonResponse = await HttpUtils.Patch(
                $"{ConfigStore.Get(ConfigKeys.ServerUri)}/users",
                new Dictionary<string, string> { 
                    { "username", Username }
                });

            if (JsonResponse.TryGetProperty("message", out var Message) && JsonResponse.TryGetProperty("error", out var Error))
            {
                return $"{Message.GetString()}: {Error}" ?? "Server response could not be extracted";
            }
            else if(JsonResponse.TryGetProperty("user", out var User))
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
        catch (Exception Error)
        {
            return $"Error encountered in updating your username or home planet : {Error.Message}";
        }
    }

    public static async Task<OneOf<string, UserModel>> UpdateUserActiveStatus(int Id, bool Active)
    {
        try
        {
            var JsonResponse = await HttpUtils.Patch(
                $"{ConfigStore.Get(ConfigKeys.ServerUri)}/users/{Id}",
                new Dictionary<string, bool> { 
                    { "active", Active }, 
                });

            if (JsonResponse.TryGetProperty("message", out var Message) && JsonResponse.TryGetProperty("error", out var Error))
            {
                return $"{Message.GetString()}: {Error}" ?? "Server response could not be extracted";
            }
            else if(JsonResponse.TryGetProperty("user", out var User))
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
        catch (Exception Error)
        {
            return $"Error encountered in updating Active status: {Error.Message}";
        }
    }

    public static async Task<OneOf<string, UserModel>> UpdateUserRole(int Id, string Role)
    {
        try
        {
            var JsonResponse = await HttpUtils.Patch(
                $"{ConfigStore.Get(ConfigKeys.ServerUri)}/users/{Id}",
                new Dictionary<string, string> { 
                    { "role", Role }, 
                });

            if (JsonResponse.TryGetProperty("message", out var Message) && JsonResponse.TryGetProperty("error", out var Error))
            {
                return $"{Message.GetString()}: {Error}" ?? "Server response could not be extracted";
            }
            else if(JsonResponse.TryGetProperty("user", out var User))
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
        catch (Exception Error)
        {
            return $"Error encountered in updating users Role: {Error.Message}";
        }
    }
}
