using AMS_Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
        private void ReportDiagnosticFake(string message)
        {

            //return;
            generatorExecutionContext.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "AMS0001",
                        "An warning by AMS generator",
                        "-->{0}<--",
                        "AMSGenerator",
                        DiagnosticSeverity.Warning,
                        isEnabledByDefault: true),
                    Location.None,
                    message ));

        }
        private GeneratorExecutionContext generatorExecutionContext;
        public void Execute(GeneratorExecutionContext context)
        {
            generatorExecutionContext = context;
            try
            {
                ExecuteInternal(context);
            }
            catch(Exception ex)
            {
                generatorExecutionContext.ReportDiagnostic(Diagnostic.Create(
        new DiagnosticDescriptor(
            "AMS0001",
            "Problem Generating source code",
            "-->{0}<--",
            "AMSGenerator",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true),
        Location.None,
        ex.Message));


            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ExecuteInternal(GeneratorExecutionContext context)
        {
            
            var ass = context.Compilation.Assembly;
            var file = ass.Locations
                
                .FirstOrDefault(it => it.Kind == LocationKind.SourceFile);
            var pathRepo = file.SourceTree.FilePath;
            pathRepo = Path.GetDirectoryName(pathRepo);

            var val = context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.AMSMerge", out var ClassAndMethod);
            MethodInfo miInvoke = null;
            if (val)
            {
                var arr = ClassAndMethod.Split('.');
                var theClass =  context.Compilation.GetSymbolsWithName(arr[0], SymbolFilter.Type).FirstOrDefault() as INamedTypeSymbol;
                if (theClass != null)
                {
                    if (theClass.Locations.Length == 1)
                    {

                        //var peReference = context.Compilation.References
                        //    .Select(it => it as PortableExecutableReference)
                        //    .Where(it => it != null)
                        //    .ToArray();
                        
                        //var neededAssemblies = new[]
                        //{
                        //        "System.Runtime",
                        //        //"mscorlib.dll",
                        //        //"System.Runtime.Extensions.dll"
                        //};
                        //string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget","packages");
                        //var referencesNeeded = neededAssemblies
                        //    .Select(it => new { folder = Path.Combine(basePath, it), file = it + ".dll" })
                        //    .SelectMany(it => Directory.GetFiles(it.folder, it.file, SearchOption.AllDirectories))
                        //    .Where(it => it.Contains("netcore"))
                        //    .Where(it => it.Contains("ref"))
                        //    .Where(it => File.Exists(it))
                        //    .Select(p => MetadataReference.CreateFromFile(p))
                        //    .ToArray();

                        var referencesNeeded =
                            new MetadataReference[]
{
    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
    MetadataReference.CreateFromFile(typeof(Regex).Assembly.Location)
};

                        //var allRefs = referencesNeeded.Union(peReference).ToArray();
                        //allRefs = referencesNeeded;
                        var nameFile = theClass.Locations.First();
                        var content = File.ReadAllText(nameFile.SourceTree.FilePath);
                        var compilation = CSharpCompilation.Create(
                   "MyDynamicAssembly.dll",
                   new[] { nameFile.SourceTree },
                   referencesNeeded,
                   new CSharpCompilationOptions(
                       OutputKind.DynamicallyLinkedLibrary,
                       optimizationLevel: OptimizationLevel.Debug)
                        );

                        using var memoryStream = new MemoryStream();
                        var result = compilation.Emit(memoryStream);
                        if (result.Success)
                        {
                            var dynamicallyCompiledAssembly = Assembly.Load(memoryStream.ToArray());
                            var exp = dynamicallyCompiledAssembly.GetExportedTypes();
                            var type = dynamicallyCompiledAssembly.GetType(theClass.ToDisplayString());
                            
                            if(type != null)
                            {
                                miInvoke= type.GetMethod(arr[1]);
                                //var m = member.Name;
                            }    
                        }
                    }
                }

            }

            var releasesVersions = GetDates(context);
            ReportDiagnosticFake("number of releases" + releasesVersions?.Length);
            var data= TryGetPropertiesFromCSPROJ(context);
            //if(!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var nameSpace))
            var nameAssembly = context.Compilation.Assembly.Name;
            
            var nameSpace = "AMS";
            AMSWithContext ams =null;
            ReleaseData[] rdAll= null;

            //rdAll = ConstructVersionsGitHub(releasesVersions,pathRepo);

            var envGithub = Environment.GetEnvironmentVariable("GITHUB_JOB");
            if (ams == null && !string.IsNullOrWhiteSpace(envGithub))
            {
                ReportDiagnosticFake("in github");

                ams = new AMSGitHub(context);
                rdAll =ConstructVersionsGitHub(releasesVersions,pathRepo);
                ReportDiagnosticFake("number of rd"+rdAll?.Length);

            }
            var envGitLab = Environment.GetEnvironmentVariable("CI_SERVER");
            if (ams == null && !string.IsNullOrWhiteSpace(envGitLab))
            {
                ams = new AMSGitLab(context);
                rdAll = ConstructVersionsGitLab(releasesVersions, pathRepo);

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
            string versioning = $"//raw commits:{rdAll?.Length}";
            ReportDiagnosticFake("rdAll.Count " + rdAll?.Length);

            if (rdAll != null)
            {
                ReportDiagnosticFake("miInvoke exists " + (miInvoke != null));

                if (miInvoke != null)
                {
                    foreach(var item in rdAll)
                    {
                        try 
                        {
                            item.Subject=miInvoke.Invoke(null, new object[] { ams.RepoUrl, item.CommitId, item.Author, item.ReleaseDate, item.Subject })?.ToString();
                        }
                        catch(Exception ex)
                        {
                            item.Subject = $"exception  {ex.Message} at {item.CommitId}";
                        }

                    }
                }
                var dict = rdAll.GroupBy(it => it.ReleaseVersion).ToDictionary(it => it.Key, it => it.ToArray());
                
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

        private ReleaseData[] ConstructVersionsGitLab(VersionReleasedAttribute[] releasesVersions,string pathRepo)
        {
            var gitMergeVersion = ConstructMergesVersionsGit( releasesVersions,pathRepo);
            if ((gitMergeVersion?.Length??0) == 0)
                return gitMergeVersion;

            return gitMergeVersion;
        }

        private ReleaseData[] ConstructVersionsGitHub(VersionReleasedAttribute[] releasesVersions,string pathRepo)
        {
            var gitMergeVersion = ConstructMergesVersionsGit(releasesVersions,pathRepo);
            if ((gitMergeVersion?.Length ?? 0) == 0)
                return gitMergeVersion;

            //gitMergeVersion = gitMergeVersion.Where(it => !it.Subject.StartsWith("into main")).ToArray();
            return gitMergeVersion;
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
            ReportDiagnosticFake("gitpath:" + gitPath);
            ReportDiagnosticFake("gitpath length" + gitPath.Length);
            return gitPath;
            //Console.WriteLine("gitPath:" + gitPath);

        }
        private ReleaseData[] ConstructMergesVersionsGit(VersionReleasedAttribute[] releasesVersions, string pathRepo)
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
                p.StartInfo.Arguments = @"log --merges --pretty=""%an|%cs|%H|%s""";
                //p.StartInfo.Arguments = "log --merges --pretty=\"\"\"%an|%cs|%H|%s\"\"\" ";
                p.OutputDataReceived += (s, e) => { output += e.Data + Environment.NewLine; };
                p.ErrorDataReceived+= (s, e) => { ReportDiagnosticFake(e.Data); };
                    p.Start();
                p.BeginOutputReadLine();

                p.WaitForExit();
                ReportDiagnosticFake("output length" + output.Length);
                ReportDiagnosticFake("output " + output);

            }


            var arr = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            ReportDiagnosticFake("arr length" + arr.Length);
            foreach (var line in arr )
            {
                var arrData = line.Split(new[] { '|' },StringSplitOptions.RemoveEmptyEntries);
                var rd = new ReleaseData();
                rd.Author=arrData[0];
                rd.ReleaseDate = DateTime.ParseExact(arrData[1], "yyyy-MM-dd",null);
                rd.CommitId = arrData[2];                
                rd.Subject =arrData[3];
                rd.ReleaseVersion = releasesVersions
                    .OrderBy(it=>it.MyDateTime())
                    .FirstOrDefault(it => it.MyDateTime().Date >=rd.ReleaseDate)
                    ?.Version();
                ReportDiagnosticFake("line " + rd.ReleaseVersion?.Name);

                if (rd.ReleaseVersion != null)
                    releases.Add(rd);
                

            }
            return releases.ToArray();
        }


        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SR());
        }
    }
}
