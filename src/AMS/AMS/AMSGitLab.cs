using AMS_Base;
using Microsoft.CodeAnalysis;
using System;

namespace AMS
{
    //https://docs.gitlab.com/ee/ci/variables/
    class AMSGitLab : AMSWithContext
    {
        public AMSGitLab(GeneratorExecutionContext context) : base(context)
        {
            CISourceControl = "GitLab";
            CommitId = Environment.GetEnvironmentVariable("CI_COMMIT_SHA", EnvironmentVariableTarget.Process);
            RepoUrl = Environment.GetEnvironmentVariable("CI_PROJECT_URL",EnvironmentVariableTarget.Process);
        }
    }
}
