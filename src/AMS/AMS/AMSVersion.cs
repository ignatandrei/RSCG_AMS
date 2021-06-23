using Microsoft.CodeAnalysis;
using System;

namespace AMS
{
    [Generator]
    public class AMSVersion : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if(!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var nameSpace))
            {
                nameSpace = "AMS";
            }
            var ams = new AMS();
            ams.AssemblyName = context.Compilation.AssemblyName;
            ams.GeneratedDate = DateTime.UtcNow;
            ams.CommitId = Environment.GetEnvironmentVariable("GITHUB_SHA");
            var classDef=$@"
using System;
namespace {nameSpace} {{ 
    public class AboutMySoftware{{
        public string AssemblyName {{ get {{ return  ""{ams.AssemblyName}"" ; }} }}
        //public DateTime DateGenerated {{ get {{ return DateTime}}
    }}
        
}}";
            context.AddSource(nameSpace + "." + ams.AssemblyName + ".cs", classDef);

        }

        public void Initialize(GeneratorInitializationContext context)
        {
            
        }
    }


    public class AMS
    {
        public string AssemblyName { get; internal set; }
        public DateTime GeneratedDate { get; internal set; }

        public string Commit { get; internal set; }
        public string CommitId { get; internal set; }
    }
}
