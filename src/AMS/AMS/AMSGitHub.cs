using Microsoft.CodeAnalysis;
using System;
using AMS_Base;
namespace AMS
{

    //https://docs.github.com/en/actions/reference/environment-variables
    class AMSGitHub : AMSWithContext
    {
        public AMSGitHub(GeneratorExecutionContext  context):base(context)
        {
            CISourceControl = "GitHub";
            CommitId = Environment.GetEnvironmentVariable("GITHUB_SHA");
            RepoUrl = Environment.GetEnvironmentVariable("GITHUB_SERVER_URL") + "/" + Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");
            SourceCommit = RepoUrl + "/tree/" + CommitId;
        }
    }
}
