using AMS_Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
[assembly: AMS_Base.VersionReleased(Name = "PreviousReleases", ISODateTime = "2022-03-31", recordData = AMS_Base.RecordData.Merges)]
[assembly: AMS_Base.VersionReleased(Name = "WithVersioning", ISODateTime = "2022-04-02", recordData = AMS_Base.RecordData.Merges)]

namespace AMSWebAPI
{

    public static class Extensions
    {
        public static IEndpointRouteBuilder UseAMS(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/ams/All", async app =>
            {
                
                   var data = AboutMySoftware.AllDefinitions;
                await app.Response.WriteAsJsonAsync(data);
            });
            endpoints.MapGet("/ams", app =>
            {//same with index.html
                var response = new ASMTemplate().Render();
                app.Response.ContentType = "text/html";
                return app.Response.WriteAsync(response);
            });
            endpoints.MapGet("/ams/index.html", app =>
            {
                var response = new ASMTemplate().Render();
                app.Response.ContentType = "text/html";
                return app.Response.WriteAsync(response);
            });
            return endpoints;


        }
    }
}
