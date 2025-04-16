using GalavisorApi.Models;
using Spectre.Console;

namespace GalavisorCli.Utils;

public static class TableBuilderUtils{
    public static Table MakeUsersTable(List<UserModel> Users){
        var DisplayTable = new Table();
        DisplayTable.AddColumn("[bold]User ID[/]");
        DisplayTable.AddColumn("[bold]Name[/]");
        DisplayTable.AddColumn("[bold]Role[/]");
        DisplayTable.AddColumn("[bold]Active[/]");

        foreach (var User in Users)
        {
            DisplayTable.AddRow(
                User.UserId.ToString(),
                User.Name,
                User.RoleName,
                User.IsActive ? "[green]Active[/]" : "[red]Inactive[/]"
            );
        }

        return DisplayTable;
    }
}