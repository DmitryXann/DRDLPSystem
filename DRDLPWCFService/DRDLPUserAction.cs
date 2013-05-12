using System;
using System.Linq;
using DRDLPSystemDAL;
using DRDLPWCFService.Helpers;
using DRDLPWCFService.Interfaces;

namespace DRDLPWCFService
{
	/// <summary>
	/// Implement document versions and document version change
	/// </summary>
	public class DRDLPUserAction : IDRDLPUserAction
	{
		private readonly int _userID;
		private readonly int _pcID;

		internal DRDLPUserAction(int pcID, int userID)
		{
			_userID = userID;
			_pcID = pcID;
		}

		public void AddNewDocument(string documentPath, string documentPart)
		{
			if (string.IsNullOrEmpty(documentPath) || string.IsNullOrEmpty(documentPart))
				return;

			using (var context = DBConnectionProvider.CreateConnection())
			{
				var selectedPC = context.GetEntityByID<PC>(_pcID);
				var selectedUser = context.GetEntityByID<User>(_userID);

				var newDocument = new Document
					{
						DocumentID = Guid.NewGuid(),
						DocumentPart = documentPart,
						LastChange = DateTime.Now,
						LastUserAccess = selectedUser,
					};

				var dbDocumentPath = new DocumentPath {Document = newDocument, Path = documentPath, PC = selectedPC};
				context.AddEntity(dbDocumentPath);

				selectedPC.DocumentPath.Add(dbDocumentPath);
				newDocument.DocumentPath.Add(dbDocumentPath);

				var documentAccess = new AccessLog
					{
						AccessDateTime = DateTime.Now,
						Document = newDocument,
						AccessType = AccessLogAccessType.Write,
						PC = selectedPC,
						Hardware = selectedPC.Hardware,
						User = selectedUser
					};

				UserAccess selectedUserAccess = null;
				var newUserAccessAdded = false;

				if (!context.GetAllEntities<UserAccess>().Any(el => el.AccessType == UserAccessAccessTypeEnum.FullAccess))
				{
					selectedUserAccess = new UserAccess { AccessType = UserAccessAccessTypeEnum.FullAccess };
					newUserAccessAdded = true;
				}

				if (!selectedUserAccess.Users.Any(el => el.Id == selectedUser.Id))
					selectedUserAccess.Users.Add(selectedUser);

				selectedUserAccess.AccessLog.Add(documentAccess);
				selectedUserAccess.Documents.Add(newDocument);

				

				documentAccess.DocumentPath = dbDocumentPath;
				documentAccess.UserAccess = selectedUserAccess;
				documentAccess.PC = selectedPC;

				dbDocumentPath.AccessLog.Add(documentAccess);
				context.AddEntity(documentAccess);
				
				selectedPC.AccessLog.Add(documentAccess);

				newDocument.UserAccess.Add(selectedUserAccess);
				newDocument.LastChange = DateTime.Now;
				newDocument.LastUserAccess = selectedUser;
				newDocument.LastUserAccessWithChanges = selectedUser;
				newDocument.PC = selectedPC;

				context.AddEntity(newDocument);

				selectedPC.Documents.Add(newDocument);

				if (newUserAccessAdded)
					context.AddEntity(selectedUserAccess);
				context.SaveChanges();
			}
		}

		public string TryOpenDocument(Guid documentID, string documentPath)
		{
			if (documentID.IsDefault() || string.IsNullOrEmpty(documentPath))
				return string.Empty;

			using (var context = DBConnectionProvider.CreateConnection())
			{
				var selectedUser = context.GetEntityByID<User>(_userID);

				var selectedUserAccess = selectedUser.UserAccesses.FirstOrDefault(el => (el.AccessType != UserAccessAccessTypeEnum.AccessDenied) &&
												el.Documents.Any(elem => elem.DocumentID.Equals(documentID)));

				if (selectedUserAccess == null)
					return string.Empty;

				var selectedDocument = selectedUserAccess.Documents.FirstOrDefault(el => el.DocumentID.Equals(documentID));

				return selectedDocument == null ? string.Empty : selectedDocument.DocumentPart;
			}
		}

		public void DocumentChanged(Guid documentID, long docuementVersion, string documentPart, string documentPath)
		{
			if (documentID.IsDefault() || string.IsNullOrEmpty(documentPart) || string.IsNullOrEmpty(documentPath))
				return;

			using (var context = DBConnectionProvider.CreateConnection())
			{
				var selectedUser = context.GetEntityByID<User>(_userID);

				var selectedUserAccess = selectedUser.UserAccesses.FirstOrDefault(el => (el.AccessType != UserAccessAccessTypeEnum.AccessDenied) &&
												el.Documents.Any(elem => elem.DocumentID.Equals(documentID)));

				if (selectedUserAccess == null)
					return;

				var selectedDocument = selectedUserAccess.Documents.FirstOrDefault(el => el.DocumentID.Equals(documentID));

				var documentVersionChange = new DocumentVersionChange
					{
						Document = selectedDocument,
						DocumentPart = selectedDocument.DocumentPart,
						Version = selectedDocument.Version
					};

				context.AddEntity(documentVersionChange);
				selectedDocument.DocumentPart = documentPart;
				selectedDocument.Version++;

				context.SaveChanges();
			}
		}
	}
}
