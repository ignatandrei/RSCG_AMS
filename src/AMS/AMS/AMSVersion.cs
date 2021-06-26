﻿using AMS_Base;
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
            var nameAssembly = context.Compilation.Assembly.Name;
            var nameSpace = "AMS";
            AMSWithContext ams =null;
            var envGithub = Environment.GetEnvironmentVariable("GITHUB_JOB");
            if (!string.IsNullOrWhiteSpace(envGithub))
            {
                ams = new AMSGitHub(context);
            }
            var envGitLab = Environment.GetEnvironmentVariable("CI_SERVER");
            if (!string.IsNullOrWhiteSpace(envGitLab))
            {
                ams = new AMSGitLab(context);
            }
            if (ams == null)
            {
                ams = new AMSWithContext(context);//default not integrated in a CI
            }
            var classDef =
$@"using System;
using AMS_Base;
namespace {nameAssembly} {{ 
    public class AboutMySoftware_{nameAssembly} :AboutMySoftware {{
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void Add_AboutMySoftware_{nameAssembly}(){{
            AboutMySoftware.AddDefinition(""{nameAssembly}"",new  AboutMySoftware_{nameAssembly}());      
        }}
        public AboutMySoftware_{nameAssembly}(){{
            AssemblyName =""{ams.AssemblyName}"" ; 
            DateGenerated = DateTime.ParseExact(""{ams.DateGenerated.ToString("yyyyMMddHHmmss")}"", ""yyyyMMddHHmmss"", null); 
            CommitId  = ""{ams.CommitId}"" ; 
            RepoUrl =""{ams.RepoUrl}"" ; 
        }}
        
    }}
        
}}";
            context.AddSource(nameSpace + "." + ams.AssemblyName + ".cs", classDef);

        }

        public void Initialize(GeneratorInitializationContext context)
        {
            
        }
    }
}
