using AMS_Base;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AMS;
[Generator]
public class AMSVersionIncremental : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var assemblyName = context.CompilationProvider.Select(static (c, _) => c.AssemblyName);
        var pathRepo = 
            context.CompilationProvider.Select(static (c, _) => c.Assembly.Locations.First(it=>it.Kind== LocationKind.SourceFile));

        var attr = context.CompilationProvider.Select(static (c, _) =>
        {
            var att = c.Assembly
                .GetAttributes()
                .Where(data => data.AttributeClass.Name == nameof(VersionReleasedAttribute))
                //.Select(it=>it.NamedArguments)                
                .ToArray();
            return att;
        });
        var argGit = context.AnalyzerConfigOptionsProvider.Select((a,_) =>
        {
            // return a.GetOptions("build_property.AMSGitArgs");
            var valMerge = a.GlobalOptions.TryGetValue($"build_property.AMSGitArgs", out var gitArgs);
            if (!valMerge)
            {
                gitArgs = @"log --merges --pretty=""%an|%cs|%H|%s""";
            }
            return gitArgs;
        });

        var data = 
            attr.
            Combine(assemblyName).
            Combine(pathRepo).
            Combine(argGit);
        context.RegisterSourceOutput(data, GenerateText);

        
    }

    private void GenerateText(SourceProductionContext arg1, (((AttributeData[] Left, string Right) Left, Location Right) Left, string Right) arg2)
    {
        GenerateText1(arg1, arg2.Left.Left.Left, arg2.Left.Left.Right, arg2.Left.Right,arg2.Right);
    }

    //private void GenerateText(SourceProductionContext arg1, ((AttributeData[] Left, string Right) Left, Location Right) arg2)
    //{
    //    GenerateText1(arg1, arg2.Left.Left,arg2.Left.Right,arg2.Right);
    //}
    private AMSWithContext ConstructAMS(string nameAssembly)
    {
        var envGithub = Environment.GetEnvironmentVariable("GITHUB_JOB");

        if (!string.IsNullOrWhiteSpace(envGithub))
        {

            return new AMSGitHub(nameAssembly);
           
        }
        var envGitLab = Environment.GetEnvironmentVariable("CI_SERVER");
        if (!string.IsNullOrWhiteSpace(envGitLab))
        {
            return new AMSGitLab(nameAssembly);

        }
        var envHeroku = Environment.GetEnvironmentVariable("DYNO");
        if (!string.IsNullOrWhiteSpace(envHeroku))
        {
            return new AMSHeroku(nameAssembly);
        }
        var envAzureDevOps = Environment.GetEnvironmentVariable("Build.BuildId");
        if (!string.IsNullOrWhiteSpace(envAzureDevOps))
        {
            return  new AMSAzureDevOps(nameAssembly);
        }
        
        return new AMSWithContext(nameAssembly);//default not integrated in a CI
        
    }
    private void GenerateText1(SourceProductionContext spc, AttributeData[]attData, string nameAssembly, Location pathRepoLocation,string gitArgs)
    {

        var assNewName = "";
        for (int i = 0; i < nameAssembly.Length; i++)
        {
            assNewName += (nameAssembly[i] + i);
        }

        var releasesVersions = CreateVersionsFromAttr(attData);
        AMSWithContext ams = new AMSGitHub(nameAssembly);        
        ReleaseData[] rdAll = null;
        var pathRepo=  pathRepoLocation.SourceTree.FilePath;
        pathRepo = Path.GetDirectoryName(pathRepo);

        rdAll = ConstructVersionsGitHub(releasesVersions, pathRepo, gitArgs);
        var dict = rdAll.GroupBy(it => it.ReleaseVersion).ToDictionary(it => it.Key, it => it.ToArray());
        string versioning = "";
        foreach (var item in dict)
        {
            var rv = item.Key;
            versioning += $@"
{{ var v=new VersionReleased();
v.Name = ""{rv.Name}"" ;
v.ISODateTime=DateTime.ParseExact(""{rv.ISODateTime.ToString("yyyyMMdd")}"",""yyyyMMdd"",null); ";
            foreach (var cm in item.Value)
            {
                versioning += $@"{{ 
var rd=new ReleaseData();
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
            User = ""{ams.User.Replace(@"\", @"\\")}"";
            IsInCI={(ams.IsInCI ? "true" : "false")};
            {versioning}
        }}
        
    }}
        
}}";
        spc.AddSource(ams.AssemblyName + ".cs", classDef);

    }
    private VersionReleasedAttribute[] CreateVersionsFromAttr(AttributeData[] attData)
    {
        List<VersionReleasedAttribute> versions = new();
        if(attData == null || attData.Length==0)
            return versions.ToArray();
        foreach (AttributeData attr in attData)
        {
            VersionReleasedAttribute v = new ();
                    
            var parameters = attr.NamedArguments;
            foreach (var parameter in parameters)
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
        return versions.ToArray();
    }

    private ReleaseData[] ConstructVersionsGitHub(VersionReleasedAttribute[] releasesVersions, string pathRepo, string gitArgs)
    {
        var gitMergeVersion = ConstructMergesVersionsGit(releasesVersions, pathRepo, gitArgs);
        if ((gitMergeVersion?.Length ?? 0) == 0)
            return gitMergeVersion;

        //gitMergeVersion = gitMergeVersion.Where(it => !it.Subject.StartsWith("into main")).ToArray();
        return gitMergeVersion;
    }
    private ReleaseData[] ConstructMergesVersionsGit(VersionReleasedAttribute[] releasesVersions, string pathRepo, string gitArgs)
    {
        if ((releasesVersions?.Length ?? 0) == 0)
            return null;

        List<ReleaseData> releases = new();
        string output = "";
        //int nr = 0;
        //while (output.Length < 10 && nr < 5)
        {
            output = "";
            //nr++;
            //ReportDiagnosticFake("starting " + nr);
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.WorkingDirectory = pathRepo;
            p.StartInfo.FileName = WhereGit();
            //p.StartInfo.Arguments = "for-each-ref --sort=committerdate refs/heads/ --format='%(authorname)|%(committerdate:short)|%(objectname)|%(refname)|%(subject)'";
            //p.StartInfo.Arguments = "for-each-ref --sort=committerdate --format='%(authorname)|%(committerdate:short)|%(objectname)|%(refname)|%(subject)'";
            //p.StartInfo.Arguments = @"log --merges --pretty=""%an|%cs|%H|%s""";
            p.StartInfo.Arguments = gitArgs;
            //p.StartInfo.Arguments = "log --merges --pretty=\"\"\"%an|%cs|%H|%s\"\"\" ";
            p.OutputDataReceived += (s, e) => { output += e.Data + Environment.NewLine; };
            //p.ErrorDataReceived += (s, e) => { ReportDiagnosticFake(e.Data); };
            p.Start();
            p.BeginOutputReadLine();

            p.WaitForExit();
            //ReportDiagnosticFake("output length" + output.Length);
            //ReportDiagnosticFake("output " + output);

        }


        var arr = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        //ReportDiagnosticFake("arr length" + arr.Length);
        foreach (var line in arr)
        {
            var arrData = line.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var rd = new ReleaseData();
            rd.Author = arrData[0];
            var strDate = arrData[1].Trim();
            if (strDate.Length > 10)
            {
                //ReportDiagnosticFake($"modify {strDate} to take just 10 chars");
                strDate = strDate.Substring(0, 10);
            }
            if (strDate.Length != 10)
            {
                //ReportDiagnosticFake($"{strDate} is invalid - should be yyyy-MM-dd");
                continue;
            }
            rd.ReleaseDate = DateTime.ParseExact(strDate, "yyyy-MM-dd", null);
            rd.CommitId = arrData[2];
            rd.Subject = arrData[3];
            rd.ReleaseVersion = releasesVersions
                .OrderBy(it => it.MyDateTime())
                .FirstOrDefault(it => it.MyDateTime().Date >= rd.ReleaseDate)
                ?.Version();
            //ReportDiagnosticFake("line " + rd.ReleaseVersion?.Name);

            if (rd.ReleaseVersion != null)
                releases.Add(rd);


        }
        return releases.ToArray();
    }

    private string WhereGit()
    {
        var p = new Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.RedirectStandardOutput = true;
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
            case PlatformID.Win32Windows:
                p.StartInfo.FileName = "where.exe";
                p.StartInfo.Arguments = "git.exe";
                break;
            case PlatformID.Unix:
            case PlatformID.MacOSX:
                p.StartInfo.FileName = "which";
                p.StartInfo.Arguments = "git";
                break;
            default:
                throw new ArgumentException("platform " + Environment.OSVersion.Platform);
        }

        string output = "";
        p.OutputDataReceived += (s, e) => { output += e.Data + Environment.NewLine; };
        p.Start();

        p.BeginOutputReadLine();
        p.WaitForExit();
        output += Environment.NewLine;
        var gitPath = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).First();
        //ReportDiagnosticFake("gitpath:" + gitPath);
        //ReportDiagnosticFake("gitpath length" + gitPath.Length);
        return gitPath;
        //Console.WriteLine("gitPath:" + gitPath);

    }
}

