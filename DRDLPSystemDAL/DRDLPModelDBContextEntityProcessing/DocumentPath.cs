using System;
using System.Collections.Generic;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddDocumentPath(DocumentPath documentPath)
		{
			if (documentPath == null)
				throw new ArgumentNullException("documentPath");
			
			if (string.IsNullOrEmpty(documentPath.Path))
				throw new ArgumentException("documetn.Path can`t b empty or null");
			
			if (documentPath.Document == null)
				throw new ArgumentNullException("documentPath.Document can`t be null");

			if (documentPath.PC == null)
				throw new ArgumentNullException("documentPath.PCs cant be null");

			if (!_container.DocumentSet.Contains(documentPath.Document))
				throw new ArgumentException("documentPath.Document do not contains in DB");

			if (!_container.PCSet.Contains(documentPath.PC))
				throw new ArgumentException("documentPath.PC do not contain in DB");

			_container.DocumentPathSet.Add(documentPath);
		}

		public void AddDocumentPath(PC pc, Document document, string path)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");

			if (document == null)
				throw new ArgumentNullException("document");

			if (string.IsNullOrEmpty(path))
				throw new ArgumentException("path can`t be empty or null");

			if (!_container.PCSet.Contains(pc))
				throw new ArgumentException("pc do not contains in DB");

			if (!_container.DocumentSet.Contains(document))
				throw new ArgumentException("document do not contains in BD");

			_container.DocumentPathSet.Add(new DocumentPath { PC = _container.PCSet.Attach(pc), Document = _container.DocumentSet.Attach(document), Path = path.ToLower().Trim() });
		}

		public DocumentPath GetDocumentPathByID(int docuemtnPathID)
		{
			return _container.DocumentPathSet.FirstOrDefault(el => el.Id == docuemtnPathID);
		}

		public IEnumerable<DocumentPath> GetAllDocumentPaths(Document document)
		{
			
			if (document == null)
				throw new ArgumentNullException("document");

			if (!_container.DocumentSet.Contains(document))
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

		public IEnumerable<DocumentPath> GetAllDocumentPaths(string documentID)
		{
			if (string.IsNullOrEmpty(documentID))
				throw new ArgumentException("documentID can`t be empty or null");

			var selectedDocument = _container.DocumentSet.FirstOrDefault(el => el.DocumentID == documentID);

			if (selectedDocument == null)
				throw new ArgumentException("document do not contains in DB");

			return _container.DocumentPathSet.Where(el => el.Document == selectedDocument).ToArray();
		}

		public long GetDocumentPathCount()
		{
			return _container.DocumentPathSet.LongCount();
		}
	}
}
