using System;
using System.Collections.Generic;
using System.Linq;

namespace AMS_Base
{
    public class AboutMySoftware
    {
        private  static Dictionary<string, AboutMySoftware> AllDefinitionsDict = new Dictionary<string, AboutMySoftware>();

        public static KeyValuePair<string,AboutMySoftware>[] AllDefinitions
        {
            get
            {
                return AllDefinitionsDict.ToArray();
            }
        }
        public static void AddDefinition(string name , AboutMySoftware soft)
        {
            lock (AllDefinitionsDict)
            {
                AllDefinitionsDict[name] = soft;
            }
        }
        public AboutMySoftware()
        {
            CommitId = "not in a CI run";
            RepoUrl = "not in a CI run";
            CISourceControl = "not in a CI run";
            SourceCommit = "#";
            DateGenerated = DateTime.UtcNow;
        }

        public string SourceCommit { get; protected set; }
        public string CISourceControl { get; protected set; }
        public string AssemblyName { get; protected set; }
        public DateTime DateGenerated { get; protected set; }
        public string CommitId { get; protected set; }
        public string RepoUrl { get; protected set; }
    }

}
