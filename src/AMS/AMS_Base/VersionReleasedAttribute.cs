using System;

namespace AMS_Base
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited = false)]
    public class VersionReleasedAttribute: Attribute
    {
        public string Name { get; set; }
        public string ISODateTime { get;set; }

        public RecordData recordData { get; set; } 
        
    }

}
