using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GalavisorCli.Constants;

namespace GalavisorCli.Utils;

public static class HttpUtils
{
    private static readonly HttpClient Client = new HttpClient();

    public static async Task<JsonElement> Get(string Url)
    {
        var Request = CreateRequest(HttpMethod.Get, Url);
        return await SendRequestAsync(Request);
    }

    public static async Task<JsonElement> Post(string Url, object RequestBody)
    {
        var Request = CreateJsonRequest(HttpMethod.Post, Url, RequestBody);
        return await SendRequestAsync(Request);
    }

    public static async Task<JsonElement> Put(string Url, object RequestBody)
    {
        var Request = CreateJsonRequest(HttpMethod.Put, Url, RequestBody);
        return await SendRequestAsync(Request);
    }

    public static async Task<JsonElement> Patch(string Url, object RequestBody)
    {
        var Request = CreateJsonRequest(HttpMethod.Patch, Url, RequestBody);
        return await SendRequestAsync(Request);
    }

    public static async Task<JsonElement> Delete(string Url)
    {
        var Request = CreateRequest(HttpMethod.Delete, Url);
        return await SendRequestAsync(Request);
    }
 
    public static async Task<JsonElement> DeleteWithBody(string Url, object RequestBody)
    {
        var Request = CreateJsonRequest(HttpMethod.Delete, Url, RequestBody);
        return await SendRequestAsync(Request);
    }

    private static HttpRequestMessage CreateRequest(HttpMethod Method, string Url)
    {
        var Request = new HttpRequestMessage(Method, Url);
        AddAuthorizationHeader(Request);
        return Request;
    }

    private static HttpRequestMessage CreateJsonRequest(HttpMethod Method, string Url, object RequestBody)
    {
        var JsonBody = JsonSerializer.Serialize(RequestBody);
        var Request = new HttpRequestMessage(Method, Url)
        {
            Content = new StringContent(JsonBody, Encoding.UTF8, "application/json")
        };
        AddAuthorizationHeader(Request);
        return Request;
    }

    private static void AddAuthorizationHeader(HttpRequestMessage Request)
    {
        try{
            string Token = ConfigStore.Get(ConfigKeys.JwtToken);
            if (!string.IsNullOrEmpty(Token))
            {
                Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            } else {
                // skip adding auth headers(most likely server will return 403 or allow if it's allowed to access the endpoint)
            }
        } catch{
            // skip adding auth headers(most likely server will return 403 or allow if it's allowed to access the endpoint)
        }
    }

    private static async Task<JsonElement> SendRequestAsync(HttpRequestMessage Request)
    {
        var Response = await Client.SendAsync(Request);
        int StatusCode = (int)Response.StatusCode;

        if (StatusCode == 401)
        {
            ConfigStore.Remove(ConfigKeys.JwtToken);
            throw new Exception("Your session has expired. Please log in again.");
        }
        else if (StatusCode == 403)
        {
            try
            {
                var JsonResponse = JsonSerializer.Deserialize<JsonElement>(await Response.Content.ReadAsStringAsync());
                if (JsonResponse.TryGetProperty("message", out var message) && JsonResponse.TryGetProperty("error", out var error))
                {
                    return JsonResponse;
                } else {
                    throw new Exception("You are not authorized to access this command");
                }
            }
            catch (Exception Error)
            {
                throw new Exception($"Could not parse JSON Response body {Error.Message}");
            }
        }
        else if (StatusCode == 404)
        {
            throw new Exception("No result returned from server.");
        }
        else if (StatusCode == 500)
        {
            throw new Exception("Internal Server Error");
        }
        else if (StatusCode >= 400)
        {
            throw new Exception($"{StatusCode} - Some error occured");
        } else{
            string responseBody = await Response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<JsonElement>(responseBody);
            }
            catch (Exception Error)
            {
                throw new Exception($"Could not parse JSON Response body {Error.Message}");
            }
        }

    }

}
