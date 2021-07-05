﻿using AMS_Base;
using Microsoft.CodeAnalysis;
using System;

namespace AMS
{
    class AMSHeroku : AMSWithContext
    {
        public AMSHeroku(GeneratorExecutionContext context) : base(context)
        {
            CISourceControl = "Heroku";
            CommitId = Environment.GetEnvironmentVariable("SOURCE_VERSION");
            var str = string.Join(",",Environment.GetEnvironmentVariables().Keys);
            RepoUrl = str;
            SourceCommit = "in heroku";
        }
    }
}