using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Papyrus.Business.Products;

namespace Papyrus.Business.Topics
{
    public class VersionRangeCollisionDetector
    {
        private readonly ProductRepository productRepository;
        private List<ProductVersion> Versions { get; set; }


        public VersionRangeCollisionDetector(ProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<bool> IsThereAnyCollisionFor(Topic topic)
        {
            Versions = await productRepository.GetAllVersionsFor(topic.ProductId);
            var versionRanges = topic.VersionRanges;
            foreach (var versionRange in versionRanges)
            {
                if (DoesVersionRangeCollideWithAnyRangeIn(versionRange, versionRanges))
                {
                    return true;
                }
            }
            return false;
        }

        private bool DoesVersionRangeCollideWithAnyRangeIn(VersionRange versionRange, VersionRanges versionRanges)
        {
            var fromVersionId = versionRange.FromVersionId;
            var isThereCollision =
                versionRanges.Where(vr => vr != versionRange)
                    .Any(vr => ReleaseFor(vr.FromVersionId) <= ReleaseFor(fromVersionId) &&
                               ReleaseFor(fromVersionId) <= ReleaseFor(vr.ToVersionId));
            if (isThereCollision)
            {
                return true;
            }
            return false;
        }

        private DateTime ReleaseFor(string versionId)
        {
            return Versions.First(vr => versionId == vr.VersionId).Release;
        }
    }
}