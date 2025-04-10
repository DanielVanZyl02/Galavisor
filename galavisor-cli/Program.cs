using GalavisorCli.App;

// docs: https://spectreconsole.net/

class Program
{
    static async Task Main(string[] args)
    {
        var app = Setup.SetupCli();
        await Run.RunCli(app);
    }
}