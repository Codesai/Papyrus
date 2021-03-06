using System;

namespace Papyrus.Business.Products
{
    public class ProductVersion {
        public string VersionId   { get; private set; }
        public string VersionName { get; set; }
        public DateTime Release { get; private set; }

        public ProductVersion(string versionId, string versionName, DateTime release)
        {
            VersionId = versionId;
            VersionName = versionName;
            Release = release;
        }

        public override bool Equals(object obj)
        {
            var otherVersion = obj as ProductVersion;
            if (otherVersion != null)
            {
                return VersionId == otherVersion.VersionId &&
                        VersionName == otherVersion.VersionName &&
                        Release.Equals(otherVersion.Release);                
            }
            return false;
        }

        public ProductVersion Clone() {
            return new ProductVersion(VersionId, VersionName, Release);
        }
    }

    public class LastProductVersion : ProductVersion {
        public const string Name = "Last version";
        public const string Id = "*";
        public LastProductVersion() : base(Id, Name, DateTime.MaxValue) {}
    }
}