using AMS_Base;
using Microsoft.CodeAnalysis;
using System;

namespace AMS
{
    class AMSHeroku : AMSWithContext
    {
        //public AMSHeroku(GeneratorExecutionContext context) : base(context)
        //{
        //    CISourceControl = "Heroku";
        //    CommitId = Environment.GetEnvironmentVariable("SOURCE_VERSION");
        //    IsInCI = true;
        //}
        public AMSHeroku(string nameAssembly) : base(nameAssembly)
        {
            CISourceControl = "Heroku";
            CommitId = Environment.GetEnvironmentVariable("SOURCE_VERSION");
            IsInCI = true;
        }

    }
}
