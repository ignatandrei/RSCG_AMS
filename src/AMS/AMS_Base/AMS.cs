using System;

namespace AMS_Base
{
    public class AMSWithContext : AboutMySoftware
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">must have  Compilation?.AssemblyName </param>
        public AMSWithContext(dynamic context):base()
        {
            AssemblyName = context?.Compilation?.AssemblyName;                        
        }
        public AMSWithContext(string assemblyName)
        {
            this.AssemblyName = assemblyName;
        }

        
    }

}
