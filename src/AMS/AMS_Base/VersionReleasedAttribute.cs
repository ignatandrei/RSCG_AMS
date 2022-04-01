using System;

namespace AMS_Base
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited = false)]
    public class VersionReleasedAttribute: Attribute
    {
        public string ISODateTime { get;set; }

        public RecordData recordData { get; set; } 
        
    }

}
