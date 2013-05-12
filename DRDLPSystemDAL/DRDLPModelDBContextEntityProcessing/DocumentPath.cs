using System;
using System.Collections.Generic;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddDocumentPath(PC pc, Document document, string path)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");

			if (document == null)
				throw new ArgumentNullException("document");

			if (string.IsNullOrEmpty(path))
				throw new ArgumentException("path can`t be empty or null");

			if (!_container.PCSet.Any(el => el.Id == pc.Id))
				throw new ArgumentException("pc do not contains in DB");

			if (!_container.DocumentSet.Any(el => el.Id == document.Id))
				throw new ArgumentException("document do not contains in BD");

			_container.DocumentPathSet.Add(new DocumentPath { PC = _container.PCSet.Attach(pc), Document = _container.DocumentSet.Attach(document), Path = path.ToLower().Trim() });
		}

		public IEnumerable<DocumentPath> GetAllDocumentPaths(Document document)
		{
			
			if (document == null)
				throw new ArgumentNullException("document");

			if (!_container.DocumentSet.Any(el => el.Id == document.Id))
				throw new ArgumentException("document do not contains in DB");

			var selectedDocument = _container.DocumentSet.Attach(document);

			return _container.DocumentPathSet.Where(el => el.Document == selectedDocument).ToArray();
		}

		public IEnumerable<DocumentPath> GetAllDocumentPaths(int documentId)
		{
			var selectedDocument = _container.DocumentSet.FirstOrDefault(el => el.Id == documentId);

			if (selectedDocument == null)
				throw new ArgumentException("document do not contains in DB");
			
			return _container.DocumentPathSet.Where(el => el.Document == selectedDocument).ToArray();
		}

		public IEnumerable<DocumentPath> GetAllDocumentPaths(Guid documentID)
		{
			if (documentID == default(Guid))
				throw new ArgumentException("documentID can`t be default GUID value");

			var selectedDocument = _container.DocumentSet.FirstOrDefault(el => el.DocumentID.CompareTo(documentID) == 0);

			if (selectedDocument == null)
				throw new ArgumentException("document do not contains in DB");

			return _container.DocumentPathSet.Where(el => el.Document == selectedDocument).ToArray();
		}
	}
}
