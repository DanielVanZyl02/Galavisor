using System.Text;
using System.Text.Json;

public static class TokenUtils
{
    private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public static string DecodeJWT(string key, string jwt)
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
