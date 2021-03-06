using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Papyrus.Business.Products;
using Papyrus.Business.Topics;

namespace Papyrus.Business.Exporters {
    public class WebsiteConstructor {
        private readonly TopicQueryRepository topicRepo;
        private WebsiteCollection websitesCollection;

        public WebsiteConstructor(TopicQueryRepository topicRepo) {
            this.topicRepo = topicRepo;
        }

        public virtual async Task<WebsiteCollection> Construct(Product product, List<string> languages)
        {
            var websites = new WebsiteCollection();
            foreach (var version in product.Versions) {
                foreach (var language in languages) {
                    var website = await CreateWebsiteWithAllDocumentsFor(product, version, language);
                    if (website.HasNotDocuments()) continue;
                    websites.Add(website);
                }
            }
            return websites;
        }

        private async Task<WebSite> CreateWebsiteWithAllDocumentsFor(Product product, ProductVersion version, string language) {
            var documents = await topicRepo.GetAllDocumentsFor(product.Id, version.VersionName, language);
            return new WebSite(documents, product.Name, language, version.VersionName);
        }
    }
}