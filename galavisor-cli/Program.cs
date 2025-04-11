using GalavisorCli.App;

class Program
{
    static async Task Main(string[] args)
    {
        var app = Setup.SetupCli();
         await Run.RunCli(app);
    }
}