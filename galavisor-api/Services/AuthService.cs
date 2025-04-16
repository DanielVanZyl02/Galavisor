using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using GalavisorApi.Constants;
using GalavisorApi.Models;
using GalavisorApi.Repositories;
using GalavisorApi.Utils;

namespace GalavisorApi.Services;

public class AuthService(HttpClient HttpClient, UserRepository UserRepository)
{
    private readonly HttpClient _httpClient = HttpClient;
    private readonly UserRepository _userRepository = UserRepository;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<string> AuthenticateUserAsync(string AuthCode)
    {
        return await GetJwtAsync(AuthCode);
    }

    public async Task<UserModel> GetOrCreateUser(string Jwt)
    {
        var Sub = DecodeJWT("sub", Jwt);
        var User = await _userRepository.GetBySub(Sub);
        if (User != null)
        {
            return User;
        }
        else
        {
            var Name = DecodeJWT("name", Jwt);
            return await _userRepository.CreateUser(Sub, Name);
        }
    }

    public async Task<int> GetLoggedInUser(string Jwt)
    {
        var Sub = DecodeJWT("sub", Jwt);
        var User = await _userRepository.GetBySub(Sub);
        if (User != null)
        {
            return User.UserId;
        }
        else
        {
            return -1;
        }
    }

    public async Task<bool> IsSubAdmin(string Sub)
    {
        var User = await _userRepository.GetBySub(Sub);
        return User != null && User.RoleName == "Admin";
    }

    private async Task<string> GetJwtAsync(string AuthCode)
    {
        var Values = new Dictionary<string, string>
        {
            ["code"] = HttpUtility.UrlDecode(AuthCode),
            ["client_id"] = ConfigStore.Get(ConfigKeys.ClientId),
            ["client_secret"] = ConfigStore.Get(ConfigKeys.ClientSecret),
            ["redirect_uri"] = ConfigStore.Get(ConfigKeys.RedirectUri),
            ["grant_type"] = "authorization_code",
            ["scope"] = "openid email profile"
        };

        var Content = new FormUrlEncodedContent(Values);
        var Request = new HttpRequestMessage(HttpMethod.Post, ConfigStore.Get(ConfigKeys.TokenUrl))
        {
            Content = Content
        };
        Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var Response = await _httpClient.SendAsync(Request);
        Response.EnsureSuccessStatusCode();

        var Json = await Response.Content.ReadAsStringAsync();
        var tokenMap = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(Json);

        return tokenMap?["id_token"].GetString() ?? "";
    }
    private static string DecodeJWT(string Key, string Jwt)
    {
        var Parts = Jwt.Split('.');
        if (Parts.Length != 3)
        {
            throw new ArgumentException("Invalid JWT token");
        }

        string PayloadJson = DecodeBase64(Parts[1]);
        var PayloadMap = JsonSerializer.Deserialize<Dictionary<string, object>>(PayloadJson, JsonOptions);

        if (PayloadMap != null && PayloadMap.TryGetValue(Key, out object? Value))
        {
            return Value?.ToString() ?? throw new Exception("Key not found in JWT payload");
        }

        throw new Exception("Key not found in JWT payload");
    }

    private static string DecodeBase64(string base64String)
    {
        string base64 = base64String
            .Replace('-', '+')
            .Replace('_', '/');

        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
            case 0: break;
            default:
                throw new FormatException("Invalid Base64URL string");
        }

        byte[] decodedBytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(decodedBytes);
    }
}
