using System.Text.Json;
using GalavisorApi.Models;
using GalavisorCli.Constants;
using  GalavisorCli.Utils;

namespace GalavisorCli.Services;

public class AuthService
{
    public static async Task<string> AttemptGoogleLoginAsync()
    {
        try
        {
            string AuthCode = await BrowserAuthServices.GetUsersGoogleAuthCodeAsync();
            if (string.IsNullOrEmpty(AuthCode))
            {
                return "Browser authentication took too long or failed, releasing resources";
            }
            else
            {
                var JsonResponse = await HttpUtils.Post(
                    $"{ConfigStore.Get(ConfigKeys.ServerUri)}/auth/login",
                    new Dictionary<string, string> { { "AuthCode", AuthCode } });

                if (JsonResponse.TryGetProperty("message", out var Message) && JsonResponse.TryGetProperty("error", out var Error)){
                    return $"{Message}, ${Error}";
                } else if(JsonResponse.TryGetProperty("message", out Message) && JsonResponse.TryGetProperty("jwt", out var Jwt)){
                    if(JsonResponse.TryGetProperty("user", out var User)){
                        var DeserializedUser = User.Deserialize<UserModel>();
                        if(DeserializedUser != null){
                            ConfigStore.Set(ConfigKeys.JwtToken, Jwt.GetString() ?? "");
                            ConfigStore.Set(ConfigKeys.GoogleName, DeserializedUser.Name);
                            return $"Authentication successful! Hi {DeserializedUser.Name}, welcome to the Galavisor App!";
                        } else{
                            return "Something went wrong whilst accessing your data, please try logging in again";
                        }
                    } else{
                        return "Authentication failed as user was not found";
                    }
                } else{
                    return "Authentication failed as jwt token was not found";
                }
            }
        }
        catch (Exception Error)
        {
            return $"Error encountered in attempting Google login: {Error.Message}";
        }
    }
}
