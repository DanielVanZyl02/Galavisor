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

                if (jsonResponse.TryGetProperty("jwt", out var jwtToken))
                {
                    ConfigStore.Set(ConfigKeys.JwtToken, jwtToken.GetString() ?? "");
                    string googleSub = TokenUtils.DecodeJWT("sub", jwtToken.GetString() ?? "");
                    ConfigStore.Set(ConfigKeys.GoogleSub, googleSub);

                    string name = TokenUtils.DecodeJWT("name", jwtToken.GetString() ?? "");
                    ConfigStore.Set(ConfigKeys.GoogleName, name);
                    return $"Authentication successful! Hi {name}, welcome to the TODO App!";
                }
                else
                {
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
