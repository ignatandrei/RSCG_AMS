using System;
using AMS_Base;
[assembly:VersionReleased(Name="PreviousReleases",ISODateTime ="2022-03-31",recordData = RecordData.Merges)]
[assembly: VersionReleased(Name = "WithVersioning", ISODateTime = "2022-04-01", recordData = RecordData.Merges)]

namespace AMSConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("Show About My Software versions");
            var amsAll = AboutMySoftware.AllDefinitions;
            Console.WriteLine("Number definitions:" + amsAll?.Length);
            foreach (var amsKV in amsAll)
            {
                var ams = amsKV.Value;

                Console.WriteLine($"{amsKV.Key}.{nameof(ams.AssemblyName)} : {ams.AssemblyName}");
                Console.WriteLine($"{amsKV.Key}.{nameof(ams.DateGenerated)} : {ams.DateGenerated}");
                Console.WriteLine($"{amsKV.Key}.{nameof(ams.CommitId)} : {ams.CommitId}");
                Console.WriteLine($"{amsKV.Key}.{nameof(ams.RepoUrl)} : {ams.RepoUrl}");
                Console.WriteLine($"{amsKV.Key}.{nameof(ams.CISourceControl)} : {ams.CISourceControl}");
                Console.WriteLine($"{amsKV.Key}.{nameof(ams.Authors)} : {ams.Authors}");
                Console.WriteLine($"{amsKV.Key}.{nameof(ams.Version)} : {ams.Version}");
                Console.WriteLine("versions" + ams.Versions?.Length);
                foreach(var item in ams.Versions)
                {
                    Console.WriteLine("release:"+item.Name);
                    foreach(var merge in item.releaseDatas)
                    {
                        Console.WriteLine("=>merge:" + merge.Subject);
                    }

                }
            }
        }
    }
}
