using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
[assembly: AMS_Base.VersionReleased(Name = "PreviousReleases", ISODateTime = "2022-03-31", recordData = AMS_Base.RecordData.Merges)]
//[assembly: AMS_Base.VersionReleased(Name = "WithVersioning", ISODateTime = "2022-04-01", recordData = AMS_Base.RecordData.Merges)]
[assembly: AMS_Base.VersionReleased(Name = "WithVersioning", ISODateTime = "2022-04-02", recordData = AMS_Base.RecordData.Merges)]

namespace AMSWebAPIVerif
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //AMSConsole.Program.Main(args);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
