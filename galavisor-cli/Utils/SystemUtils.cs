using System.Diagnostics;

namespace GalavisorCli.Utils;

public static class SystemUtils{
    public static void OpenBrowser(string url)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        catch (Exception e)
        {
            Console.WriteLine("Unable to open browser: " + e.Message);
        }
    }
}