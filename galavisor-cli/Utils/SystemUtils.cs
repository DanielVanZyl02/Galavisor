using System.Diagnostics;

namespace GalavisorCli.Utils;

public static class SystemUtils{
    public static void OpenBrowser(string Url)
    {
        try
        {
            var ProcessStartInfomation = new ProcessStartInfo
            {
                FileName = Url,
                UseShellExecute = true
            };
            Process.Start(ProcessStartInfomation);
        }
        catch (Exception Error)
        {
            Console.WriteLine("Unable to open browser because: " + Error);
        }
    }
}