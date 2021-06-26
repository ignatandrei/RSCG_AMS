using System;
using System.Collections.Generic;

namespace AMS_Base
{
    public class AboutMySoftware
    {
        public static Dictionary<string, AboutMySoftware> AllDefinitions = new Dictionary<string, AboutMySoftware>();
        public string AssemblyName { get; protected set; }
        public DateTime DateGenerated { get; protected set; }
        public string CommitId { get; protected set; }
        public string RepoUrl { get; protected set; }
    }

}
