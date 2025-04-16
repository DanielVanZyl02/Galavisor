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
            string authCode = await BrowserAuthServices.GetUsersGoogleAuthCodeAsync();
            if (string.IsNullOrEmpty(authCode))
            {
                return "Browser authentication took too long or failed, releasing resources";
            }
            else
            {
                var jsonResponse = await HttpUtils.Post(
                    $"{ConfigStore.Get(ConfigKeys.ServerUri)}/auth/login",
                    new Dictionary<string, string> { { "AuthCode", authCode } });

                if (jsonResponse.TryGetProperty("message", out var message) && jsonResponse.TryGetProperty("error", out var error)){
                    return $"{message}, ${error}";
                } else if(jsonResponse.TryGetProperty("message", out message) && jsonResponse.TryGetProperty("jwt", out var jwt)){
                    if(jsonResponse.TryGetProperty("user", out var User)){
                        var DeserializedUser = User.Deserialize<UserModel>();
                        if(DeserializedUser != null){
                            ConfigStore.Set(ConfigKeys.JwtToken, jwt.GetString() ?? "");
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
        catch (Exception error)
        {
            return $"Error encountered in attempting Google login: {error.Message}";
        }
    }
}
