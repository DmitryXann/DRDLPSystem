using System;
using System.Collections.Generic;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddDocument(Document document)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			if (string.IsNullOrEmpty(document.DocumentID))
				throw new ArgumentException("document.DocumentID can`t be empty or null");

			if (document.PC == null)
				throw new ArgumentNullException("document.PC");

			if (document.DocumentPath == null)
				throw new ArgumentNullException("document.DocumentPath");

			if (document.LastUserAccess == null)
				throw new ArgumentNullException("document.LastUserAccess");

			if (document.LastUserAccessWithChanges == null)
				throw new ArgumentNullException("document.LastUserAccessWithChanges");

			if (document.UserAccess == null)
				throw new ArgumentNullException("document.UserAccess");

			if (!document.DocumentPath.Any())
				throw new ArgumentException("document.DocumentPath can`t be empty");

			if (!document.UserAccess.Any())
				throw new ArgumentException("document.DocumentPath can`t be empty");

			if (!document.DocumentPath.All(el => _container.DocumentPathSet.Contains(el)))
				throw new ArgumentException("all document.DocumentPath must contains in DB");

			if (!document.UserAccess.All(el => _container.UserAccessSet.Contains(el)))
				throw new ArgumentException("all document.DocumentPath must contains in DB");

			if (document.LastChange == DateTime.MinValue)
				document.LastChange = DateTime.Now;

			_container.DocumentSet.Add(document);
		}

		public void RemoveDocument(Document document)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			_container.DocumentSet.Remove(_container.DocumentSet.Attach(document));
		}

		public Document AttachDocument(Document document)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			return _container.DocumentSet.Attach(document);
		}

		public void ChangeDocumentVersion(Document document, long documentVersion)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			if (!_container.DocumentSet.Contains(document))
				throw new ArgumentException("document do not contains in DB");

			document = _container.DocumentSet.Attach(document);

			_container.DocumentSet.FirstOrDefault(el => el == document).Version = documentVersion;
		}


		public void ChangeDocumentInfo(Document document, User lastUserAccess, User lastUserAccessWithChanges = null, DocumentPath documentPath = null)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			if ((lastUserAccess == null) && (lastUserAccessWithChanges == null) && (documentPath == null))
				throw new ArgumentNullException("All lastUserAccess && lastUserAccessWithChanges && documentPath params can`t be null");

			if ((documentPath != null) && !_container.DocumentPathSet.Contains(documentPath))
				throw new ArgumentException("documentPath do not contains in DB");

			var selectedDocument = _container.DocumentSet.FirstOrDefault(el => el == _container.DocumentSet.Attach(document));

			if (selectedDocument == null)
				throw new ArgumentException("document do not contains in DB");

			if (lastUserAccess != null)
				selectedDocument.LastUserAccess = _container.UserSet.Attach(lastUserAccess);

			if (lastUserAccessWithChanges != null)
				selectedDocument.LastUserAccessWithChanges = _container.UserSet.Attach(lastUserAccessWithChanges);


			if ((documentPath != null) && !selectedDocument.DocumentPath.Contains(documentPath))
				selectedDocument.DocumentPath.Add(_container.DocumentPathSet.Attach(documentPath));
		}

		public void ChangeDocumentInfo(int documentID, User lastUserAccess, User lastUserAccessWithChanges = null, DocumentPath documentPath = null)
		{
			if ((lastUserAccess == null) && (lastUserAccessWithChanges == null) && (documentPath == null))
				throw new ArgumentNullException("All lastUserAccess && lastUserAccessWithChanges && documentPath params can`t be null");

			if ((documentPath != null) && !_container.DocumentPathSet.Contains(documentPath))
				throw new ArgumentException("documentPath do not contains in DB");

			var selectedDocument = _container.DocumentSet.FirstOrDefault(el => el.Id == documentID);

			if (selectedDocument == null)
				throw new ArgumentException("document do not contains in DB");

			if (lastUserAccess != null)
				selectedDocument.LastUserAccess = _container.UserSet.Attach(lastUserAccess);

			if (lastUserAccessWithChanges != null)
				selectedDocument.LastUserAccessWithChanges = _container.UserSet.Attach(lastUserAccessWithChanges);


			if ((documentPath != null) && !selectedDocument.DocumentPath.Contains(documentPath))
				selectedDocument.DocumentPath.Add(_container.DocumentPathSet.Attach(documentPath));
		}
		
		public Document GetDocumentByID(int docuemtnId)
		{
			return _container.DocumentSet.FirstOrDefault(el => el.Id == docuemtnId);
		}

		public IEnumerable<Document> GetAllDocumentsByDocumentID(string documentID)
		{
			if (string.IsNullOrEmpty(documentID))
				throw new ArgumentException("document id can`t be empty or null");

			return _container.DocumentSet.Where(el => el.DocumentID == documentID).ToArray();
		}

		public IEnumerable<Document> GetAllDocuments()
		{
			return _container.DocumentSet.ToArray();
		} 

		public long GetDocumentCount()
		{
			return _container.DocumentSet.LongCount();
		}
	}
}
