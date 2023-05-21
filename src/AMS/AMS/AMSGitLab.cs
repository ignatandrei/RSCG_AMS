using AMS_Base;
using Microsoft.CodeAnalysis;
using System;

namespace AMS
{
    //https://docs.gitlab.com/ee/ci/variables/
    class AMSGitLab : AMSGit
    {
        //public AMSGitLab(GeneratorExecutionContext context) : base(context)
        //{
        //    CISourceControl = "GitLab";
        //    CommitId = Environment.GetEnvironmentVariable("CI_COMMIT_SHA", EnvironmentVariableTarget.Process);
        //    RepoUrl = Environment.GetEnvironmentVariable("CI_PROJECT_URL",EnvironmentVariableTarget.Process);
        //    SourceCommit = RepoUrl + "/-/tree/" + CommitId;
        //    IsInCI = true;
        //}
        public AMSGitLab(string nameAssembly) : base(nameAssembly)
        {
            CISourceControl = "GitLab";
            CommitId = Environment.GetEnvironmentVariable("CI_COMMIT_SHA", EnvironmentVariableTarget.Process);
            RepoUrl = Environment.GetEnvironmentVariable("CI_PROJECT_URL", EnvironmentVariableTarget.Process);
            SourceCommit = RepoUrl + "/-/tree/" + CommitId;
            IsInCI = true;
        }
    }
}
