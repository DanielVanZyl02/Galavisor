using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using GalavisorApi.Constants;
using GalavisorApi.Models;
using GalavisorApi.Repositories;

namespace GalavisorApi.Services;

public class AuthService(HttpClient httpClient, UserRepository userRepository)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly UserRepository _userRepository = userRepository;
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<string> AuthenticateUserAsync(string authCode)
    {
        return await GetJwtAsync(authCode);
    }

    public async Task<UserModel> GetOrCreateUser(string jwt)
    {
        var sub = DecodeJWT("sub", jwt);
        var User = await _userRepository.GetBySub(sub);
        if(User != null){
            return User;
        } else{
            var name = DecodeJWT("name", jwt);
            return await _userRepository.CreateUser(sub, name);
        }
    }
  
    public async Task<bool> IsSubAdmin(string sub){
        var User = await _userRepository.GetBySub(sub);
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
    private static string DecodeJWT(string key, string jwt)
    {
        var parts = jwt.Split('.');
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
        byte[] decodedBytes = Convert.FromBase64String(base64String);
        return Encoding.UTF8.GetString(decodedBytes);
    }
}
