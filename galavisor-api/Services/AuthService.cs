using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;
using GalavisorApi.Constants;
using GalavisorApi.Repositories;

namespace GalavisorApi.Services;

public class AuthService(HttpClient httpClient, IConfiguration config, UserRepository userRepository)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _config = config;

    private readonly UserRepository _userRepository = userRepository;

    public async Task<string> AuthenticateUserAsync(string authCode)
    {
        string jwt = await GetJwtAsync(authCode);

        var jwtResponse = new Dictionary<string, string> { ["jwt"] = jwt };
        return JsonSerializer.Serialize(jwtResponse);
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

    /*private User DecodeJwt(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length != 3)
            throw new ArgumentException("Invalid JWT token");

        string payload = parts[1];
        byte[] jsonBytes = Convert.FromBase64String(PadBase64(payload));
        string json = Encoding.UTF8.GetString(jsonBytes);

        var payloadMap = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        var sub = payloadMap["sub"].ToString();
        var email = payloadMap["email"].ToString();
        var name = payloadMap["name"].ToString();

        return new User(sub, name, email);
    }

    private string PadBase64(string base64)
    {
        return base64.Length % 4 switch
        {
            2 => base64 + "==",
            3 => base64 + "=",
            _ => base64
        };
    }*/
}
