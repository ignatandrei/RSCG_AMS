using Microsoft.CodeAnalysis;
using System;

namespace AMS
{
    [Generator]
    public class AMSVersion : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            //if(!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var nameSpace))
            
            var nameSpace = "AMS";
            AMS ams=null;
            var envGithub = Environment.GetEnvironmentVariable("GITHUB_JOB");
            if (!string.IsNullOrWhiteSpace(envGithub))
            {
                ams = new AMSGitHub(context);
            }
            var envGitLab = Environment.GetEnvironmentVariable("CI_JOB_ID");
            if (!string.IsNullOrWhiteSpace(envGitLab))
            {
                ams = new AMSGitHub(context);
            }
            if (ams == null)
            {
                ams = new AMS(context);//default not integrated in a CI
            }
            var classDef =$@"
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
