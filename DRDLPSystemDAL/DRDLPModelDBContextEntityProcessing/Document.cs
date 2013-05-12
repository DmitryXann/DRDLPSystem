using System;
using System.Collections.Generic;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void ChangeDocumentVersion(Document document, long documentVersion)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			if (!_container.DocumentSet.Any(el => el.Id == document.Id))
				throw new ArgumentException("document do not contains in DB");

			document = _container.DocumentSet.Attach(document);

			_container.DocumentSet.FirstOrDefault(el => el == document).Version = documentVersion;
		}


		public void ChangeDocumentInfo(Document document, User lastUserAccess, User lastUserAccessWithChanges = null, DocumentPath documentPath = null, string documentPart = null)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			if ((lastUserAccess == null) && (lastUserAccessWithChanges == null) && (documentPath == null))
				throw new ArgumentNullException("All lastUserAccess && lastUserAccessWithChanges && documentPath params can`t be null");

			if ((documentPath != null) && !_container.DocumentPathSet.Any(el => el.Id == documentPath.Id))
				throw new ArgumentException("documentPath do not contains in DB");

			var selectedDocument = _container.DocumentSet.FirstOrDefault(el => el == _container.DocumentSet.Attach(document));

			if (selectedDocument == null)
				throw new ArgumentException("document do not contains in DB");

			if (lastUserAccess != null)
				selectedDocument.LastUserAccess = _container.UserSet.Attach(lastUserAccess);

			if (lastUserAccessWithChanges != null)
				selectedDocument.LastUserAccessWithChanges = _container.UserSet.Attach(lastUserAccessWithChanges);

			if (!string.IsNullOrEmpty(documentPart))
				selectedDocument.DocumentPart = documentPart;

			if ((documentPath != null) && !selectedDocument.DocumentPath.Any(el => el.Id == documentPath.Id))
				selectedDocument.DocumentPath.Add(_container.DocumentPathSet.Attach(documentPath));
		}

		public void ChangeDocumentInfo(int documentID, User lastUserAccess, string documentPart, User lastUserAccessWithChanges = null, DocumentPath documentPath = null)
		{
			if ((lastUserAccess == null) && (lastUserAccessWithChanges == null) && (documentPath == null))
				throw new ArgumentNullException("All lastUserAccess && lastUserAccessWithChanges && documentPath params can`t be null");

			if ((documentPath != null) && !_container.DocumentPathSet.Any(el => el.Id == documentPath.Id))
				throw new ArgumentException("documentPath do not contains in DB");

			var selectedDocument = _container.DocumentSet.FirstOrDefault(el => el.Id == documentID);

			if (selectedDocument == null)
				throw new ArgumentException("document do not contains in DB");

			if (string.IsNullOrEmpty(documentPart))
				throw new ArgumentException("documentPart can`t be empty or null");

			selectedDocument.DocumentPart = documentPart;

			if (lastUserAccess != null)
				selectedDocument.LastUserAccess = _container.UserSet.Attach(lastUserAccess);

			if (lastUserAccessWithChanges != null)
				selectedDocument.LastUserAccessWithChanges = _container.UserSet.Attach(lastUserAccessWithChanges);


			if ((documentPath != null) && !selectedDocument.DocumentPath.Any(el => el.Id == documentPath.Id))
				selectedDocument.DocumentPath.Add(_container.DocumentPathSet.Attach(documentPath));
		}
		
		public IEnumerable<Document> GetAllDocumentsByDocumentID(Guid documentID)
		{
			if (documentID == default (Guid))
				throw new ArgumentException("documentID can`t be default GUID value");

			return _container.DocumentSet.Where(el => el.DocumentID.CompareTo(documentID) == 0).ToArray();
		}

		public string GetDoucumentPart(Guid documentID, User accessingUser, out UserAccessAccessTypeEnum accessAccess)
		{
			if (documentID == default(Guid))
				throw new ArgumentException("documentID can`t be default GUID value");

			var selectedDocument = _container.DocumentSet.FirstOrDefault(el => el.DocumentID.CompareTo(documentID) == 0);

			if (selectedDocument == null)
				throw new ArgumentException(string.Format("Document with {0} documrntID not found", documentID));

			if ((accessingUser.UserAccesses == null) || accessingUser.UserAccesses.Any())
				throw new ArgumentException("accessingUser do not have UserAccesses data");

			var selectedUserAccess = selectedDocument.UserAccess.FirstOrDefault(el => el.Users.Any(elem => elem.Id == accessingUser.Id));

			if ((selectedUserAccess == null) || (selectedUserAccess.AccessType == UserAccessAccessTypeEnum.AccessDenied))
			{
				accessAccess = UserAccessAccessTypeEnum.AccessDenied;
				return string.Empty;
			}
			
			accessAccess = selectedUserAccess.AccessType;
			return selectedDocument.DocumentPart;
		}
	}
}
