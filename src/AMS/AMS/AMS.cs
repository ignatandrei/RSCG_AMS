using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace AMS
{
    public class AboutMySoftware
    {
        public static Dictionary<string, AboutMySoftware> AllDefinitions=new Dictionary<string, AboutMySoftware>(); 
        public string AssemblyName { get; protected set; }
        public DateTime DateGenerated { get; protected set; } 
        public string CommitId { get; protected set; }
        public string RepoUrl { get; protected set; }
    }

    class AMS: AboutMySoftware
    {
        public AMS(GeneratorExecutionContext  context)
        {
            AssemblyName = context.Compilation.AssemblyName;
            DateGenerated = DateTime.UtcNow;
            CommitId = "not in a CI run";
            RepoUrl = "not in a CI run";
        }
    }
}
