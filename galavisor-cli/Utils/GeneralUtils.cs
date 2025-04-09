using GalavisorCli.Constants;

namespace GalavisorCli.Utils;

public static class GeneralUtils{
    public static string[] getKnownCommands()
    {
        return [ 
            CommandsConstants.login, 
            CommandsConstants.logout, 
            CommandsConstants.add, 
            CommandsConstants.list, 
            CommandsConstants.update, 
            CommandsConstants.delete, 
            CommandsConstants.exit, 
            CommandsConstants.help 
        ];
    }
}