using System;
using AMS;
namespace AMSConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Show About My Software versions");
            var ams = new AboutMySoftware();
            Console.WriteLine($"{nameof(ams.AssemblyName)} : {ams.AssemblyName}");
            Console.WriteLine($"{nameof(ams.DateGenerated)} : {ams.DateGenerated}");
            Console.WriteLine($"{nameof(ams.CommitId)} : {ams.CommitId}");
            Console.WriteLine($"{nameof(ams.RepoUrl)} : {ams.RepoUrl}");
        }
    }
}
