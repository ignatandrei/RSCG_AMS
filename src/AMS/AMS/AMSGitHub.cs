using Microsoft.CodeAnalysis;
using System;

namespace AMS
{
    class AMSGitHub : AMS
    {
        public AMSGitHub(GeneratorExecutionContext  context):base(context)
        {
            CommitId = Environment.GetEnvironmentVariable("GITHUB_SHA");
            RepoUrl = Environment.GetEnvironmentVariable("GITHUB_SERVER_URL") + "/" + Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");
        }
    }
}
