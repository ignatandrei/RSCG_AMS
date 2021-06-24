﻿using AMS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;

partial class ASMTemplate
{
    StringBuilder sb = new StringBuilder();
    public void WriteText(string text)
    {
        sb.AppendLine(text);
    }
    public void WriteValue(string text)
    {
        sb.AppendLine(text);
    }
    public string Render()
    {
        this.RenderCore();
        return sb.ToString();
    }
}
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
            endpoints.MapGet("/ams/index", app =>
            {
                var response = new ASMTemplate().Render();
                return app.Response.WriteAsync(response);
            });
            return endpoints;


        }
    }
}
