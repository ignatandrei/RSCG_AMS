using AMS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AMSWebAPI
{
    public static class Extensions
    {
        public static IEndpointRouteBuilder UseAMS(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/ams/All", async app =>
            {

                var data = AboutMySoftware.AllDefinitions.Select(it => it).ToArray();
                await app.Response.WriteAsJsonAsync(data);
            });            
            return endpoints;
        }
    }
}
