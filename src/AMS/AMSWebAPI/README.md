# RSCG_AMS
a Roslyn Source Code Generator for About My Software

## For  Web applications

Add to the csproj
```xml 
    <PackageReference Include="AMSWebAPI" Version="2021.6.27.452" />
    <PackageReference Include="AMS_Base" Version="2021.6.27.452" />
    <PackageReference Include="RSCG_AMS" Version="2021.6.27.452" ReferenceOutputAssembly="false" OutputItemType="Analyzer" />

```

And in the Startup.cs put

```csharp
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.UseAMS();
});
```

The access /ams/all ( for json)  or /ams/index.html ( for html)