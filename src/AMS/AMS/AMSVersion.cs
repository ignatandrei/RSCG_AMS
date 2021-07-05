using AMS_Base;
using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Xml;

namespace AMS
{
    class ItemsFromCSPROJ
    {
        public string Authors { get; set; }
        public string Version { get; internal set; }
    }
    [Generator]
    public class AMSVersion : ISourceGenerator
    {
        private ItemsFromCSPROJ TryGetPropertiesFromCSPROJ(GeneratorExecutionContext context)
        {
            var ret= new ItemsFromCSPROJ();
            try
            {
                var dirFolder = ((dynamic)(context.Compilation)).Options?.SourceReferenceResolver?.BaseDirectory;
                if (string.IsNullOrWhiteSpace(dirFolder))
                    return ret;

                var file = Directory.GetFiles(dirFolder, "*.csproj");
                if (file.Length != 1)
                    throw new ArgumentException($"find files at {dirFolder} :{file.Length} ");

                var xmldoc = new XmlDocument();
                xmldoc.Load(file[0]);
                XmlNode node;
                node = xmldoc.SelectSingleNode("//Authors");
                ret.Authors = node?.InnerText;
                node = xmldoc.SelectSingleNode("//Version");
                ret.Version = node?.InnerText;
                return ret;
            }
            catch(Exception )
            {
                //maybe log warning? 
                return ret;
            }

        }
        public void Execute(GeneratorExecutionContext context)
        {
            var data= TryGetPropertiesFromCSPROJ(context);
            //if(!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var nameSpace))
            var nameAssembly = context.Compilation.Assembly.Name;
            
            var nameSpace = "AMS";
            AMSWithContext ams =null;
            var envGithub = Environment.GetEnvironmentVariable("GITHUB_JOB");
            if (ams == null && !string.IsNullOrWhiteSpace(envGithub))
            {
                ams = new AMSGitHub(context);
            }
            var envGitLab = Environment.GetEnvironmentVariable("CI_SERVER");
            if (ams == null && !string.IsNullOrWhiteSpace(envGitLab))
            {
                ams = new AMSGitLab(context);
            }
            var envHeroku= Environment.GetEnvironmentVariable("DYNO");
            if (ams == null && !string.IsNullOrWhiteSpace(envHeroku))
            {
                ams = new AMSHeroku(context);
            }
            if (ams == null)
            {
                ams = new AMSWithContext(context);//default not integrated in a CI
            }
            //dealing with  <PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="5.0.7" />

            var assNewName = "";
            for (int i = 0; i < nameAssembly.Length; i++)
            {
                assNewName += (nameAssembly[i] +i) ;
            }
            ams.Authors = data.Authors;
            ams.Version = data.Version;
            var classDef =
$@"using System;
using AMS_Base;
namespace {nameAssembly} {{ 
    /// <summary>
    /// this is the About My Software for {assNewName}
    /// </summary>
    public class XAboutMySoftware_{assNewName} :AboutMySoftware {{
        /// <summary>
        /// starts when this module is loaded and 
        /// add the AMS tot the 
        /// </summary>
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void Add_AboutMySoftware_{assNewName}(){{
            AboutMySoftware.AddDefinition(""{nameAssembly}"",new  XAboutMySoftware_{assNewName}());      
        }}
        /// <summary>
        /// constructor
        /// for AMS {assNewName}
        /// </summary>
        public XAboutMySoftware_{assNewName}(){{
            AssemblyName =""{ams.AssemblyName}"" ; 
            DateGenerated = DateTime.ParseExact(""{ams.DateGenerated.ToString("yyyyMMddHHmmss")}"", ""yyyyMMddHHmmss"", null); 
            CommitId  = ""{ams.CommitId}"" ; 
            RepoUrl =""{ams.RepoUrl}"" ; 
            CISourceControl = ""{ams.CISourceControl}"" ; 
            SourceCommit = ""{ams.SourceCommit}"" ; 
            Authors= ""{ams.Authors}"";
            Version= ""{ams.Version}"";    

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
