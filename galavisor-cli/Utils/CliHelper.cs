using System.Text.RegularExpressions;

namespace GalavisorCli.Utils;

public static partial class CliHelper
{
    public static string[] ShellSplit(string input)
    {
        var matches = MyRegex().Matches(input);
        return matches.Select(m => m.Value.Trim('"')).ToArray();
    }

    public static string SuggestCommand(string attempted, string[] knownCommands)
    {
        return knownCommands
            .OrderBy(cmd => LevenshteinDistance(cmd, attempted))
            .FirstOrDefault() ?? "help";
    }

    public static int LevenshteinDistance(string a, string b)
    {
        var dp = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) dp[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                var cost = a[i - 1] == b[j - 1] ? 0 : 1;
                dp[i, j] = new[] {
                    dp[i - 1, j] + 1,
                    dp[i, j - 1] + 1,
                    dp[i - 1, j - 1] + cost
                }.Min();
            }
        }

        return dp[a.Length, b.Length];
    }

    [GeneratedRegex(@"[\""].+?[\""]|\S+")]
    private static partial Regex MyRegex();
}
