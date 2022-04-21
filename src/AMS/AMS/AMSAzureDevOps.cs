using Microsoft.CodeAnalysis;
using System;
using AMS_Base;
namespace AMS
{

    //https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=azure-devops&tabs=yaml
    class AMSAzureDevOps : AMSWithContext
    {
        public AMSAzureDevOps(GeneratorExecutionContext  context):base(context)
        {
            CISourceControl = "AzureDevOps";
            CommitId = Environment.GetEnvironmentVariable("Build.SourceVersion");
            RepoUrl = Environment.GetEnvironmentVariable("Build.Repository.Uri");
            IsInCI = true;
            //SourceCommit = RepoUrl + "/tree/" + CommitId; // how to find initial repo type ?
        }
    }
}
