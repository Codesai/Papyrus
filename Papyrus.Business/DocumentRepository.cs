namespace Papyrus.Business
{
    using System.Collections.Generic;

    public interface DocumentRepository
    {
        void Save(Document document);
        Document GetDocument(string id);
        void Update(Document document);
        void Delete(string documentId);
        IEnumerable<Document> GetAllDocuments();
    }
}