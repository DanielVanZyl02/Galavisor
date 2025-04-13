using GalavisorCli.Constants;

namespace GalavisorCli.Utils;

public static class GeneralUtils{
    public static string[] GetKnownCommands()
    {
        return [ 
            CommandsConstants.login, 
            CommandsConstants.logout, 
            CommandsConstants.config, 
            CommandsConstants.add, 
            CommandsConstants.list, 
            CommandsConstants.update, 
            CommandsConstants.delete, 
            CommandsConstants.exit, 
            CommandsConstants.help,
            CommandsConstants.review,
            CommandsConstants.getreview,
            CommandsConstants.updatereview,
            CommandsConstants.deletereview
            CommandsConstants.planets,
            CommandsConstants.getplanet,
            CommandsConstants.getweather,
            CommandsConstants.addplanet,
            CommandsConstants.updateplanet
            CommandsConstants.addactivity,
            CommandsConstants.getactivity,
            CommandsConstants.updateactivity,
            CommandsConstants.deleteactivity,
            CommandsConstants.linkactivity

        ];
    }
}