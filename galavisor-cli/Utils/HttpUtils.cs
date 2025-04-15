using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GalavisorCli.Constants;

namespace GalavisorCli.Utils;

public static class HttpUtils
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<JsonElement> Get(string url)
    {
        var request = CreateRequest(HttpMethod.Get, url);
        return await SendRequestAsync(request);
    }

    public static async Task<JsonElement> Post(string url, object requestBody)
    {
        var request = CreateJsonRequest(HttpMethod.Post, url, requestBody);
        return await SendRequestAsync(request);
    }

    public static async Task<JsonElement> Put(string url, object requestBody)
    {
        var request = CreateJsonRequest(HttpMethod.Put, url, requestBody);
        return await SendRequestAsync(request);
    }

    public static async Task<JsonElement> Patch(string url, object requestBody)
    {
        var request = CreateJsonRequest(HttpMethod.Patch, url, requestBody);
        return await SendRequestAsync(request);
    }

    public static async Task<JsonElement> Delete(string url)
    {
        var request = CreateRequest(HttpMethod.Delete, url);
        return await SendRequestAsync(request);
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        AddAuthorizationHeader(request);
        return request;
    }

    private static HttpRequestMessage CreateJsonRequest(HttpMethod method, string url, object requestBody)
    {
        var jsonBody = JsonSerializer.Serialize(requestBody);
        var request = new HttpRequestMessage(method, url)
        {
            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
        };
        AddAuthorizationHeader(request);
        return request;
    }

    private static void AddAuthorizationHeader(HttpRequestMessage request)
    {
        try{
            string token = ConfigStore.Get(ConfigKeys.JwtToken);
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        } catch{
            // skip adding auth headers(most likely server will return 401 or allow if it's allowed to access the endpoint)
        }
    }

    private static async Task<JsonElement> SendRequestAsync(HttpRequestMessage request)
    {
        var response = await client.SendAsync(request);
        int statusCode = (int)response.StatusCode;

        if (statusCode == 401)
        {
            
            // Handle session expiration
            ConfigStore.Remove(ConfigKeys.JwtToken);
            throw new Exception("Your session has expired. Please log in again.");
        }
        else if (statusCode == 404)
        {
            throw new Exception("Resource not found.");
        }
        else if (statusCode >= 400)
        {
            throw new Exception($"{statusCode} - {await response.Content.ReadAsStringAsync()}");
        }

        string responseBody = await response.Content.ReadAsStringAsync();
        try
        {
            return JsonSerializer.Deserialize<JsonElement>(responseBody);
        }
        catch (Exception ex)
        {
            throw new Exception($"Could not parse JSON response body {ex.Message}");
        }
    }

    public static async Task<JsonElement> DeleteWithBody(string url, object requestBody)
    {
        var request = CreateJsonRequest(HttpMethod.Delete, url, requestBody);
        return await SendRequestAsync(request);
    }

}
