using System;

namespace AMS_Base
{
    public class ReleaseData
    {
        public string Subject { get; set; }
        public string Author { get; set; }
        public DateTime ReleaseDate { get; set; }

        public string CommitId { get; set; }

        public VersionReleased ReleaseVersion { get; set; }

    }
}
