using GalavisorCli.Constants;

namespace GalavisorCli.Utils;

public static class GeneralUtils{
    public static string[] GetKnownCommands()
    {
        return [ 
            // Auth
            CommandsConstants.Login, 
            CommandsConstants.Logout, 

            // Users
            CommandsConstants.Config,
            CommandsConstants.Config,
            CommandsConstants.Disable,
            CommandsConstants.Enable,
            CommandsConstants.Role,
            CommandsConstants.Users,
            
            CommandsConstants.Exit, 
            CommandsConstants.Help,
            CommandsConstants.review,
            CommandsConstants.getreview,
            CommandsConstants.updatereview,
            CommandsConstants.deletereview,
            CommandsConstants.planets,
            CommandsConstants.getplanet,
            CommandsConstants.getweather,
            CommandsConstants.addplanet,
            CommandsConstants.updateplanet,
            CommandsConstants.deleteplanet,
            CommandsConstants.updateplanet,
            CommandsConstants.AddActivity,
            CommandsConstants.GetActivity,
            CommandsConstants.UpdateActivity,
            CommandsConstants.DeleteActivity,
            CommandsConstants.LinkActivity,
            CommandsConstants.AddTransport,
            CommandsConstants.GetTransport,
            CommandsConstants.UpdateTransport,
            CommandsConstants.DeleteTransport,
            CommandsConstants.LinkTransport
        ];
    }
}