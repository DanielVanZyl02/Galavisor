using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using GalavisorApi.Constants;
using GalavisorApi.Models;
using GalavisorApi.Repositories;

namespace GalavisorApi.Services;

public class AuthService(HttpClient HttpClient, UserRepository UserRepository)
{
    private readonly HttpClient _httpClient = HttpClient;
    private readonly UserRepository _userRepository = UserRepository;
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<string> AuthenticateUserAsync(string authCode)
    {
        return await GetJwtAsync(authCode);
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

    private async Task<string> GetJwtAsync(string authCode)
    {
        var values = new Dictionary<string, string>
        {
            ["code"] = HttpUtility.UrlDecode(authCode),
            ["client_id"] = ConfigStore.Get(ConfigKeys.ClientId),
            ["client_secret"] = ConfigStore.Get(ConfigKeys.ClientSecret),
            ["redirect_uri"] = ConfigStore.Get(ConfigKeys.RedirectUri),
            ["grant_type"] = "authorization_code",
            ["scope"] = "openid email profile"
        };

        var content = new FormUrlEncodedContent(values);
        var request = new HttpRequestMessage(HttpMethod.Post, ConfigStore.Get(ConfigKeys.TokenUrl))
        {
            Content = content
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var tokenMap = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        return tokenMap?["id_token"].GetString() ?? "";
    }
    private static string DecodeJWT(string key, string Jwt)
    {
        var parts = Jwt.Split('.');
        if (parts.Length != 3)
        {
            throw new ArgumentException("Invalid JWT token");
        }

        string payloadJson = DecodeBase64(parts[1]);
        var payloadMap = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson, jsonOptions);

        if (payloadMap != null && payloadMap.TryGetValue(key, out object? value))
        {
            return value?.ToString() ?? throw new Exception("Key not found in JWT payload");
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
