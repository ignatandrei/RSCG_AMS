using AMS_Base;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private VersionReleasedAttribute[] GetDates(GeneratorExecutionContext context)
        {
            List<VersionReleasedAttribute> versions = new();
            //maybe get from class SR : ISyntaxReceiver ?
            var verAttr = context.Compilation.Assembly.GetAttributes();
            foreach(var attr in verAttr)
            {
                if(attr.AttributeClass.Name == nameof(VersionReleasedAttribute))
                {
                    var v = new VersionReleasedAttribute();
                    var parameters = attr.NamedArguments;
                    foreach(var parameter in parameters)
                    {
                        var val = parameter.Value.Value.ToString();
                        switch (parameter.Key)
                        {
                            case "Name":
                                v.Name = val;
                                break;
                            case "ISODateTime":
                                v.ISODateTime = val;
                                break;
                            case "recordData":
                                v.recordData = (RecordData)(int.Parse(val));
                                break;
                        }
                    }
                    versions.Add(v);
                }
            }
            return versions.ToArray();
        }
        public void Execute(GeneratorExecutionContext context)
        {

            var releasesVersions = GetDates(context);
            
            var data= TryGetPropertiesFromCSPROJ(context);
            //if(!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var nameSpace))
            var nameAssembly = context.Compilation.Assembly.Name;
            
            var nameSpace = "AMS";
            AMSWithContext ams =null;
            ReleaseData[] rd= null;

            //rd = ConstructVersionsGitHub(releasesVersions);

            var envGithub = Environment.GetEnvironmentVariable("GITHUB_JOB");
            if (ams == null && !string.IsNullOrWhiteSpace(envGithub))
            {
                ams = new AMSGitHub(context);
                rd =ConstructVersionsGitHub(releasesVersions);
            }
            var envGitLab = Environment.GetEnvironmentVariable("CI_SERVER");
            if (ams == null && !string.IsNullOrWhiteSpace(envGitLab))
            {
                ams = new AMSGitLab(context);
                rd = ConstructVersionsGitLab(releasesVersions);

            }
            var envHeroku= Environment.GetEnvironmentVariable("DYNO");
            if (ams == null && !string.IsNullOrWhiteSpace(envHeroku))
            {
                ams = new AMSHeroku(context);
            }
            var envAzureDevOps = Environment.GetEnvironmentVariable("Build.BuildId");
            if (ams == null && !string.IsNullOrWhiteSpace(envAzureDevOps))
            {
                ams = new AMSAzureDevOps(context);
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
            //versioning
            string versioning = $"//raw commits:{rd?.Length}";
            if(rd != null)
            {
                var dict = rd.GroupBy(it => it.ReleaseVersion).ToDictionary(it => it.Key, it => it.ToArray());
                
                foreach (var item in dict)
                {
                    var rv = item.Key;
                    versioning += $@"
{{ var v=new VersionReleased();
v.Name = ""{rv.Name}"" ;
v.ISODateTime=DateTime.ParseExact(""{rv.ISODateTime.ToString("yyyyMMdd")}"",""yyyyMMdd"",null); ";
                    foreach(var cm in item.Value)
                    {
                        versioning += $@"{{ 
var rd=new ReleaseData();
rd.Branch = ""{cm.Branch}"";
rd.Author = ""{cm.Author}"";
rd.CommitId = ""{cm.CommitId}"";
rd.Subject = ""{cm.Subject}"";
rd.ReleaseDate = DateTime.ParseExact(""{cm.ReleaseDate.ToString("yyyyMMdd")}"",""yyyyMMdd"",null);  
v.AddRelease(rd);
";
                        
                        versioning += "}";
                    }
                    versioning += " this.AddVersion(v);";
                    versioning += "}";
                
                }
            }
            

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
            EnvironmentVars =""{ams.EnvironmentVars}"";
            User = ""{ams.User.Replace(@"\",@"\\")}"";
            {versioning}
        }}
        
    }}
        
}}";
            context.AddSource(nameSpace + "." + ams.AssemblyName + ".cs", classDef);

        }

        private ReleaseData[] ConstructVersionsGitLab(VersionReleasedAttribute[] releasesVersions)
        {
            var gitBranchVersion = ConstructBranchVersionsGit( releasesVersions);
            if ((gitBranchVersion?.Length??0) == 0)
                return gitBranchVersion;

            return gitBranchVersion;
        }

        private ReleaseData[] ConstructVersionsGitHub(VersionReleasedAttribute[]  releasesVersions)
        {
            var gitBranchVersion = ConstructBranchVersionsGit(releasesVersions);
            if ((gitBranchVersion?.Length ?? 0) == 0)
                return gitBranchVersion;

            gitBranchVersion = gitBranchVersion.Where(it => !it.Subject.StartsWith("Merge pull request")).ToArray();
            return gitBranchVersion;
        }
        private string WhereGit()
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32Windows:
                    p.StartInfo.FileName = "where.exe";
                    break;
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    p.StartInfo.FileName = "which";
                    break;
                default:
                    throw new ArgumentException("platform " + Environment.OSVersion.Platform);
            }

            p.StartInfo.Arguments = "git.exe";
            string output = "";
            p.OutputDataReceived += (s, e) => { output += e.Data + Environment.NewLine; };
            p.Start();

            p.BeginOutputReadLine();
            p.WaitForExit();
            output += Environment.NewLine;
            var gitPath = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).First();
            return gitPath;
            //Console.WriteLine("gitPath:" + gitPath);

        }
        private ReleaseData[] ConstructBranchVersionsGit(VersionReleasedAttribute[] releasesVersions)
        {
            if ((releasesVersions?.Length ?? 0) == 0)
                return null;

            List<ReleaseData> releases = new();
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = WhereGit();
            //p.StartInfo.Arguments = "for-each-ref --sort=committerdate refs/heads/ --format='%(authorname)|%(committerdate:short)|%(objectname)|%(refname)|%(subject)'";
            p.StartInfo.Arguments = "for-each-ref --sort=committerdate --format='%(authorname)|%(committerdate:short)|%(objectname)|%(refname)|%(subject)'";
            string output = "";
            p.OutputDataReceived += (s, e) => { output += e.Data + Environment.NewLine; };
            p.Start();
            
            p.BeginOutputReadLine();
            p.WaitForExit();
            foreach(var line in output.Split(new[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries))
            {
                var arrData = line.Split(new[] { '|' },StringSplitOptions.RemoveEmptyEntries);
                var rd = new ReleaseData();
                rd.Author=arrData[0];
                rd.ReleaseDate = DateTime.ParseExact(arrData[1], "yyyy-MM-dd",null);
                rd.CommitId = arrData[2];
                rd.Branch = arrData[3];
                rd.Subject =arrData[4];
                rd.ReleaseVersion = releasesVersions.First(it => it.MyDateTime().Date <=rd.ReleaseDate).Version();
                releases.Add(rd);
                

            }
            return releases.ToArray();
        }


        public void Initialize(GeneratorInitializationContext context)
        {
            //context.RegisterForSyntaxNotifications(() => new SR());
        }
    }
}
