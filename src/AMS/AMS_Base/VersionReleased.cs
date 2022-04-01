using System;
using System.Collections.Generic;

namespace AMS_Base
{
    public class VersionReleased
    {
        private List<ReleaseData> backup;
        public VersionReleased()
        {
            backup = new List<ReleaseData>();
        }
        public VersionReleased(VersionReleasedAttribute attr):base()
        {
            this.Name = attr.Name;
            this.ISODateTime = attr.MyDateTime();
        }
        public void AddRelease(ReleaseData releaseData)
        {
            this.backup.Add(releaseData);
        }

        public ReleaseData[] releaseDatas { 
            get
            {
                return backup.ToArray();
            }
            set
            {
                backup = new List<ReleaseData>(value);
            }
        }
        public string Name { get; set; }
        public DateTime ISODateTime { get; set; }

        
    }
}
