using Microsoft.CodeAnalysis;

namespace AMS
{
    [Generator]
    public class AMSVersion : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            //if(!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var nameSpace))
            
            var nameSpace = "AMS";            
            var ams = new AMSGitHub(context);
            var classDef=$@"
using System;
namespace {nameSpace} {{ 
    public class AboutMySoftware{{
        public string AssemblyName {{ get {{ return  ""{ams.AssemblyName}"" ; }} }}
        public DateTime DateGenerated {{ get {{ return DateTime.ParseExact(""{ams.GeneratedDate.ToString("yyyyMMddHHmmss")}"", ""yyyyMMddHHmmss"", null); }} }}
        public string CommitId  {{ get {{ return  ""{ams.CommitId}"" ; }}}}
        public string RepoUrl {{ get {{ return  ""{ams.RepoUrl}"" ; }}}}
    }}
        
}}";
            context.AddSource(nameSpace + "." + ams.AssemblyName + ".cs", classDef);

        }

        public void Initialize(GeneratorInitializationContext context)
        {
            
        }
    }
}
