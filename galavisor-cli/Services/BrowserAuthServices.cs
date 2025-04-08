using System.Net;
using System.Text;
using GalavisorCli.Constants;

namespace GalavisorCli.Services;

public static class BrowserAuthServices
{
    public static async Task<string> GetUsersGoogleAuthCodeAsync()
    {
        var tcs = new TaskCompletionSource<string>();
        var listener = new HttpListener();
        listener.Prefixes.Add(ConfigStore.Get(ConfigKeys.RedirectUri) + "/");
        listener.Start();

        // 🔗 Build login URL
        string loginUrl = $"{ConfigStore.Get(ConfigKeys.AuthUrl)}" +
                          $"?client_id={ConfigStore.Get(ConfigKeys.ClientId)}" +
                          $"&redirect_uri={Uri.EscapeDataString(ConfigStore.Get(ConfigKeys.RedirectUri))}" +
                          $"&response_type=code" +
                          $"&scope=openid%20email%20profile";
        SystemUtils.OpenBrowser(loginUrl);
        Console.WriteLine("Waiting for Google authentication...");

        _ = Task.Run(async () =>
        {
            try
            {
                var context = await listener.GetContextAsync();
                var request = context.Request;

                if (!request.Url!.Query.Contains("code="))
                {
                    tcs.TrySetException(new Exception("Authorization failed"));
                    return;
                }

                string query = request.Url.Query;
                string code = query.Split("code=")[1].Split('&')[0];

                string responseText = "Authentication successful! You can close this tab.";
                var buffer = Encoding.UTF8.GetBytes(responseText);
                var response = context.Response;
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer);
                response.Close();

                tcs.TrySetResult(code);
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }
            finally
            {
                listener.Stop();
            }
        });

        // Timeout after 60 seconds
        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(int.Parse(ConfigStore.Get(ConfigKeys.localAuthTimeout))));
        if (completedTask != tcs.Task)
        {
            Console.WriteLine("Login timed out.");
            return "";
        }

        var authCode = await tcs.Task;

        return authCode;
    }
}
