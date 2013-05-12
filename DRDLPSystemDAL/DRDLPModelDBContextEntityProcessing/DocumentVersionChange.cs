using System;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddDocumentVersionChange(Document document, long version, string documentPart)
		{
			if (document == null)
				throw new AggregateException("document");

			if (string.IsNullOrEmpty(documentPart))
				throw new ArgumentException("documentPart can`t be empty or null");

			if (!_container.DocumentSet.Any(el => el.Id == document.Id))
				throw new AggregateException("document do not contains in DB");

			_container.DocumentVersionChangeSet.Add(new DocumentVersionChange
				{
					Document = document,
					Version = version,
					DocumentPart = documentPart.ToLower().Trim()
				});
		}

		public string GetSpecificDocumentVersionPart(Guid documentID, long version)
		{
			var selectedDocument = _container.DocumentSet.FirstOrDefault(el => el.DocumentID.Equals(documentID));

			if (selectedDocument == null)
				throw new ArgumentException(string.Format("document with {0} guid do not contains in db", documentID));

			var selectedDocumentVersion = selectedDocument.DocumentVersionChange.FirstOrDefault(el => el.Version == version);

			return selectedDocumentVersion == null ? string.Empty : selectedDocumentVersion.DocumentPart;
		}
	}
}
