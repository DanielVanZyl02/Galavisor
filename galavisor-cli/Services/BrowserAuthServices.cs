using System.Net;
using System.Text;
using GalavisorCli.Constants;
using  GalavisorCli.Utils;
using Spectre.Console;

namespace GalavisorCli.Services;

public static class BrowserAuthServices
{
    public static async Task<string> GetUsersGoogleAuthCodeAsync()
    {
        var Tcs = new TaskCompletionSource<string>();
        var Listener = new HttpListener();
        Listener.Prefixes.Add(ConfigStore.Get(ConfigKeys.RedirectUri) + "/");
        Listener.Start();

        string LoginUrl = $"{ConfigStore.Get(ConfigKeys.AuthUrl)}" +
                          $"?client_id={ConfigStore.Get(ConfigKeys.ClientId)}" +
                          $"&redirect_uri={Uri.EscapeDataString(ConfigStore.Get(ConfigKeys.RedirectUri))}" +
                          $"&response_type=code" +
                          $"&scope=openid%20email%20profile";
        SystemUtils.OpenBrowser(LoginUrl);
        AnsiConsole.WriteLine("[green]Waiting for Google authentication...[/]");

        _ = Task.Run(async () => //What happens if you remove the _
        {
            try
            {
                var Context = await Listener.GetContextAsync();
                var Request = Context.Request;

                if (!Request.Url!.Query.Contains("code="))
                {
                    Tcs.TrySetException(new Exception("Authorization failed"));
                    return;
                } else{
                    string Query = Request.Url.Query;
                    string Code = Query.Split("code=")[1].Split('&')[0];

                    string responseText = "Authentication successful! You can close this tab.";
                    var Buffer = Encoding.UTF8.GetBytes(responseText);
                    var Response = Context.Response;
                    Response.ContentLength64 = Buffer.Length;
                    await Response.OutputStream.WriteAsync(Buffer);
                    Response.Close();

                    Tcs.TrySetResult(Code);
                }
            }
            catch (Exception Error)
            {
                Tcs.TrySetException(Error);
            }
            finally
            {
                Listener.Stop();
            }
        });

        var CompletedTask = await Task.WhenAny(Tcs.Task, Task.Delay(int.Parse(ConfigStore.Get(ConfigKeys.LocalAuthTimeout))));
        if (CompletedTask != Tcs.Task)
        {
            AnsiConsole.WriteLine("[yellow]Login timed out. Please try again[/]");
            return "";
        } else {
            return await Tcs.Task;
        }
    }
}
