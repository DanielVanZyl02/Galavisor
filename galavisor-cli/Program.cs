using GalavisorCli.App;

class Program
{
    static async Task Main(string[] args)
    {
        var App = Setup.SetupCli();
         await Run.RunCli(App);
    }
}