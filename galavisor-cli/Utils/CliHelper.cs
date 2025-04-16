using System.Text.RegularExpressions;

namespace GalavisorCli.Utils;

public static partial class CliHelper
{
    public static string[] ShellSplit(string Input)
    {
        var matches = MyRegex().Matches(Input);
        return [.. matches.Select(m => m.Value.Trim('"'))];
    }

    public static string SuggestCommand(string Attempted, string[] KnownCommands)
    {
        return KnownCommands
            .OrderBy(cmd => LevenshteinDistance(cmd, Attempted))
            .FirstOrDefault() ?? "help";
    }

    public static int LevenshteinDistance(string StringToCompareAgainst, string StringToCompareWith)
    {
        var Dp = new int[StringToCompareAgainst.Length + 1, StringToCompareWith.Length + 1];
        for (int I = 0; I <= StringToCompareAgainst.Length; I++) Dp[I, 0] = I;
        for (int J = 0; J <= StringToCompareWith.Length; J++) Dp[0, J] = J;

        for (int I = 1; I <= StringToCompareAgainst.Length; I++)
        {
            for (int J = 1; J <= StringToCompareWith.Length; J++)
            {
                var Cost = StringToCompareAgainst[I - 1] == StringToCompareWith[J - 1] ? 0 : 1;
                Dp[I, J] = new[] {
                    Dp[I - 1, J] + 1,
                    Dp[I, J - 1] + 1,
                    Dp[I - 1, J - 1] + Cost
                }.Min();
            }
        }

        return Dp[StringToCompareAgainst.Length, StringToCompareWith.Length];
    }

    [GeneratedRegex(@"[\""].+?[\""]|\S+")]
    private static partial Regex MyRegex();
}
