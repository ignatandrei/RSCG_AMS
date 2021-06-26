using System;

namespace AMS_Base
{
    public class AMSWithContext : AboutMySoftware
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">must have  Compilation?.AssemblyName </param>
        public AMSWithContext(dynamic context)
        {
            AssemblyName = context?.Compilation?.AssemblyName;
            DateGenerated = DateTime.UtcNow;
            CommitId = "not in a CI run";
            RepoUrl = "not in a CI run";
        }
    }

}
