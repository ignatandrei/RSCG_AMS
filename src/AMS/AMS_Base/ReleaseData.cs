using System;

namespace AMS_Base
{
    public class ReleaseData
    {
        public string Subject { get; set; }
        public string Author { get; set; }
        public string Branch { get; set; }
        public DateTime ReleaseDate { get; set; }

        public string CommitId { get; set; }

        public VersionReleased ReleaseVersion { get; set; }

    }

    public class VersionReleased
    {
        public VersionReleased(VersionReleasedAttribute attr)
        {
            this.Name = attr.Name;
            this.ISODateTime = attr.MyDateTime();
        }

        public ReleaseData[] releaseDatas { get; set; }
        public string Name { get; set; }
        public DateTime ISODateTime { get; set; }

        
    }
}
