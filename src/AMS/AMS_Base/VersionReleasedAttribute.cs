using System;

namespace AMS_Base
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited = false)]
    public class VersionReleasedAttribute: Attribute
    {
    
        public string Name { get; set; }
        public string ISODateTime { get;set; }

        public RecordData recordData { get; set; }
        private DateTime? cacheIsoDateTime;
        public DateTime MyDateTime()
        {
            if(cacheIsoDateTime == null)
            {
                cacheIsoDateTime= DateTime.ParseExact(ISODateTime, "yyyy-MM-dd", null);
            }
            return cacheIsoDateTime.Value;
        }
        private VersionReleased backup;
        public VersionReleased Version()
        {
            if (backup == null)
                backup = new VersionReleased(this);
            return backup;
        }

    }

}
