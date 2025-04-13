using GalavisorCli.Constants;

namespace GalavisorCli.Utils;

public static class GeneralUtils{
    public static string[] GetKnownCommands()
    {
        return [ 
            // Auth
            CommandsConstants.login, 
            CommandsConstants.logout, 

            // Users
            CommandsConstants.config,
            CommandsConstants.config,
            CommandsConstants.disable,
            CommandsConstants.enable,
            CommandsConstants.role,
            CommandsConstants.users,

            CommandsConstants.add, 
            CommandsConstants.list, 
            CommandsConstants.update, 
            CommandsConstants.delete, 
            CommandsConstants.exit, 
            CommandsConstants.help,
            CommandsConstants.review,
            CommandsConstants.getreview
        ];
    }
}