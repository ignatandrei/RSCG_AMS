using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AMSConsole
{
    public class AMSInterpreter
    {
        public static string MergeAnalyzer(string RepoUrl,string CommitId, string Author,    DateTime ReleaseDate, string Subject)
        {
            try
            {
                {
                    string regex = @"^Merge\ pull\ request\ \#(?<issueID>\d+)\ from\ (?<branch>.+?)$";

                    RegexOptions options = RegexOptions.Multiline;
                    string input = Subject;

                    MatchCollection matches = Regex.Matches(input, regex, options);
                    foreach (Match match in matches)
                    {
                        return $"Issue ID: {RepoUrl}/issues/{match.Groups["issueID"].Value} branch: {match.Groups["branch"].Value}"; ; 
                    }
                    return Subject;
                }
            }
            catch (Exception ex)
            {
                return $"{CommitId} {ex.Message}";
            }
            
        }
    }
}
