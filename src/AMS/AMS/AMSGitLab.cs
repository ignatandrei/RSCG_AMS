using Microsoft.CodeAnalysis;
using System;

namespace AMS
{
    //https://docs.gitlab.com/ee/ci/variables/
    class AMSGitLab : AMS
    {
        public AMSGitLab(GeneratorExecutionContext context) : base(context)
        {
            CommitId = Environment.GetEnvironmentVariable("CI_COMMIT_SHA");
            RepoUrl = Environment.GetEnvironmentVariable("CI_PROJECT_URL");
        }
    }
}
