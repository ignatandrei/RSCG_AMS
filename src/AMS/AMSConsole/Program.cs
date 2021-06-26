using System;
using AMS_Base;

namespace AMSConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Show About My Software versions");
            var amsAll = AboutMySoftware.AllDefinitions;
            foreach (var amsKV in amsAll)
            {
                var ams = amsKV.Value;

                Console.WriteLine($"{amsKV.Key}.{nameof(ams.AssemblyName)} : {ams.AssemblyName}");
                Console.WriteLine($"{amsKV.Key}.{nameof(ams.DateGenerated)} : {ams.DateGenerated}");
                Console.WriteLine($"{amsKV.Key}.{nameof(ams.CommitId)} : {ams.CommitId}");
                Console.WriteLine($"{amsKV.Key}.{nameof(ams.RepoUrl)} : {ams.RepoUrl}");
                Console.WriteLine($"{amsKV.Key}.{nameof(ams.CISourceControl)} : {ams.CISourceControl}");
            }
        }
    }
}
