using System;
using AMS;
namespace AMSConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var ams = new AboutMySoftware();
            Console.Write(ams.AssemblyName);
        }
    }
}
