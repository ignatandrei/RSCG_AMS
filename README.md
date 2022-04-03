# RSCG_AMS
a Roslyn Source Code Generator for About My Software

You will obtain

![RSCG_AMS](https://ignatandrei.github.io/RSCG_AMS/result.png "RSCG_AMS Generated")

( See online at https://netcoreblockly.herokuapp.com/ams )

[![AMS_BASE](https://img.shields.io/nuget/v/AMS_Base?label=AMS_Base)](https://www.nuget.org/packages/AMS_Base/)
[![RSCG_AMS](https://img.shields.io/nuget/v/RSCG_AMS?label=RSCG_AMS)](https://www.nuget.org/packages/RSCG_AMS/)
[![AMSWebAPI](https://img.shields.io/nuget/v/AMSWebAPI?label=AMSWebAPI)](https://www.nuget.org/packages/AMSWebAPI/)
 

[![BuildAndTest](https://github.com/ignatandrei/RSCG_AMS/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ignatandrei/RSCG_AMS/actions/workflows/dotnet.yml)
 2022.4.2.2037
## How to use
### For Console or DLL 

Add to the csproj 2022.4.2.2037
```xml 
<ItemGroup>
    <PackageReference Include="AMS_Base" Version=" 2022.4.2.2037" />
    <PackageReference Include="RSCG_AMS" Version=" 2022.4.2.2037" ReferenceOutputAssembly="false" OutputItemType="Analyzer" />
  </ItemGroup>
```

And access like this:
```csharp
 var amsAll = AboutMySoftware.AllDefinitions;
foreach (var amsKV in amsAll)
{
    var ams = amsKV.Value;

    Console.WriteLine($"{amsKV.Key}.{nameof(ams.AssemblyName)} : {ams.AssemblyName}");
    Console.WriteLine($"{amsKV.Key}.{nameof(ams.DateGenerated)} : {ams.DateGenerated}");
    Console.WriteLine($"{amsKV.Key}.{nameof(ams.CommitId)} : {ams.CommitId}");
    Console.WriteLine($"{amsKV.Key}.{nameof(ams.RepoUrl)} : {ams.RepoUrl}");
}
```

### For  Web applications

Add to the csproj
```xml 
    <PackageReference Include="AMSWebAPI" Version=" 2022.4.2.2037" />
    <PackageReference Include="AMS_Base" Version=" 2022.4.2.2037" />
    <PackageReference Include="RSCG_AMS" Version=" 2022.4.2.2037" ReferenceOutputAssembly="false" OutputItemType="Analyzer" />

```

And in the Startup.cs put 

```csharp
//above the namespace : using AMSWebAPI;
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.UseAMS();
});
```

The access /ams/all ( for json)  or /ams/index ( for html)

## Adding releases on date

For adding releases between dates  use the following codes:

```csharp
using AMS_Base;
[assembly:VersionReleased(Name="PreviousReleases",ISODateTime ="2022-03-31",recordData = RecordData.Merges)]
[assembly: VersionReleased(Name = "WithVersioning", ISODateTime = "2022-04-02", recordData = RecordData.Merges)]
[assembly: AMS_Base.VersionReleased(Name = "FutureRelease", ISODateTime = "9999-04-16", recordData = AMS_Base.RecordData.Merges)]

```

## How it is built

The AMS_Base project / nuget is containing the definition

The RSCG_AMS project / nuget generates the code for having , in CI , the C# class with the commit / Repo / date / other details.

The AMSWebAPI project / nuget generates the code for endpoints  :  /ams/index.html and /ams/all ( for json )

See more at http://msprogrammer.serviciipeweb.ro/category/ams/

