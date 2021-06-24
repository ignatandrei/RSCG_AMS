using Microsoft.CodeAnalysis;
using System;

namespace AMS
{
    abstract class AMS
    {
        public AMS(GeneratorExecutionContext  context)
        {
            AssemblyName = context.Compilation.AssemblyName;
            GeneratedDate = DateTime.UtcNow;

        }
        public string AssemblyName { get; internal set; }
        public DateTime GeneratedDate { get; internal set; }

        public string CommitId { get; internal set; }
        public string RepoUrl { get; internal set; }
    }
}
