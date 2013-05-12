using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddEntity<T>(T entity) where T : class
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			if (entity is AccessLog)
			{
				var accessLog = entity as AccessLog;

				if (accessLog.DocumentPath == null)
					throw new ArgumentNullException("accessLog.DocumentPath");

				if (accessLog.Document == null)
					throw new ArgumentNullException("accessLog.Document");

				if (accessLog.Hardware == null)
					throw new ArgumentNullException("accessLog.Hardware");

				if (accessLog.PC == null)
					throw new ArgumentNullException("accessLog.PC");

				if (accessLog.User == null)
					throw new ArgumentNullException("accessLog.User");

				if (accessLog.UserAccess == null)
					throw new ArgumentNullException("accessLog.UserAccess");

				if (accessLog.AccessDateTime == DateTime.MinValue)
					accessLog.AccessDateTime = DateTime.Now;

				_container.AccessLogSet.Add(accessLog);
				return;
			}

			if (entity is Administrator)
			{
				var administrator = entity as Administrator;

				if (_container.AdministratorSet.Any(el => el.Id == administrator.Id))
					throw new ArgumentException("DB already contains this administrator");

				administrator.Login = administrator.Login.ToLower().Trim();
				administrator.Password = GetSha512Sum(administrator.Password.ToLower().Trim());

				_container.AdministratorSet.Add(administrator);
				return;
			}

			if (entity is Document)
			{
				var document = entity as Document;

				if (document.DocumentID == default(Guid))
					throw new ArgumentException("documentID can`t be default GUID value");

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
					throw new ArgumentException("document.UserAccess can`t be empty");

				//if (!document.UserAccess.All(el => _container.UserAccessSet.Any(elem => elem.AccessType == el.AccessType)))
				//	throw new ArgumentException("all document.UserAccessSet must contains in DB");

				if (string.IsNullOrEmpty(document.DocumentPart))
					throw new ArgumentException("document.DocumentPart can`t be empty or null");

				if (document.LastChange == DateTime.MinValue)
					document.LastChange = DateTime.Now;

				_container.DocumentSet.Add(document);
				return;
			}

			if (entity is DocumentVersionChange)
			{
				var documentVersionChange = entity as DocumentVersionChange;

				if (documentVersionChange.Document == null)
					throw new ArgumentNullException("documentVersionChange.Document");

				if (string.IsNullOrEmpty(documentVersionChange.DocumentPart))
					throw new ArgumentException("documentVersionChange.DocumentPart can`t be empty or null");

				_container.DocumentVersionChangeSet.Add(documentVersionChange);
				return;
			}

			if (entity is DocumentPath)
			{
				var documentPath = entity as DocumentPath;

				if (string.IsNullOrEmpty(documentPath.Path))
					throw new ArgumentException("documetn.Path can`t b empty or null");

				if (documentPath.Document == null)
					throw new ArgumentNullException("documentPath.Document can`t be null");

				if (documentPath.PC == null)
					throw new ArgumentNullException("documentPath.PCs cant be null");

				//if (!_container.DocumentSet.Any(el => el.Id == documentPath.Document.Id))
				//	throw new ArgumentException("documentPath.Document do not contains in DB");

				if (!_container.PCSet.Any(el => el.Id == documentPath.PC.Id))
					throw new ArgumentException("documentPath.PC do not contain in DB");

				_container.DocumentPathSet.Add(documentPath);
				return;
			}

			if (entity is Hardware)
			{
				var hardware = entity as Hardware;

				if (hardware.PC == null)
					throw new ArgumentNullException("hardware.PC");

				hardware.HardwareID = hardware.HardwareID.ToLower().Trim();
				hardware.Name = hardware.Name.ToLower().Trim();
				hardware.OtherInfo = hardware.OtherInfo.ToLower().Trim();
				hardware.PC = _container.PCSet.Attach(hardware.PC);

				_container.HardwareSet.Add(hardware);
				return;
			}

			if (entity is PC)
			{
				var pc = entity as PC;

				if (string.IsNullOrEmpty(pc.PCHardwareBasedID))
					throw new ArgumentException("pc.PCHardwareBasedID can`t be empty or null");

				if (string.IsNullOrEmpty(pc.Name))
					throw new ArgumentException("pc.Name can`t be empty or null");

				if (pc.Hardware == null)
					throw new ArgumentNullException("pc.Hardware");

				if (!pc.Hardware.Any())
					throw new ArgumentException("pc.Hardware cant be empty");

				_container.PCSet.Add(pc);
				return;
			}

			if (entity is User)
			{
				var user = entity as User;

				if (user.PC == null)
					throw new ArgumentNullException("user.PC");

				if (_container.UserSet.Any(el => el.Id == user.Id))
					throw new ArgumentException("DB already contains this user");

				user.Login = user.Login.ToLower().Trim();

				_container.UserSet.Add(user);
				return;
			}

			if (entity is UserAccess)
			{
				var userAccess = entity as UserAccess;

				if (_container.UserAccessSet.Any(el => el.AccessType == userAccess.AccessType))
					throw new ArgumentException("Selected user access type already exist in DB");

				_container.UserAccessSet.Add(userAccess);

				return;
			}

			throw new StrongTypingException(string.Format("{0} is not correct type", typeof(T)));
		}

		public void RemoveEntity<T>(T entity) where T : class
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			if (entity is Administrator)
			{
				var administrator = entity as Administrator;

				if (!_container.AdministratorSet.Any(el => el.Id == administrator.Id))
					throw new ArgumentException("administrator do not contains in DB");

				_container.AdministratorSet.Remove(_container.AdministratorSet.Attach(administrator));
				return;
			}

			if (entity is Document)
			{
				var document = entity as Document;

				if (!_container.DocumentSet.Any(el => el.Id == document.Id))
					throw new ArgumentException("document do not contains in DB");

				_container.DocumentSet.Remove(_container.DocumentSet.Attach(document));
				return;
			}

			if (entity is DocumentVersionChange)
			{
				var documentVersionChange = entity as DocumentVersionChange;

				if (!_container.DocumentVersionChangeSet.Any(el => el.Id == documentVersionChange.Id))
					throw new ArgumentException("document do not contains in DB");

				_container.DocumentVersionChangeSet.Remove(documentVersionChange);
			}

			if (entity is DocumentPath)
			{
				var documentPath = entity as DocumentPath;

				if (!_container.DocumentPathSet.Any(el => el.Id == documentPath.Id))
					throw new ArgumentException("documentPath do not contains in DB");

				_container.DocumentPathSet.Remove(_container.DocumentPathSet.Attach(documentPath));

				return;
			}

			if (entity is Hardware)
			{
				var hardware = entity as Hardware;

				if (!_container.HardwareSet.Any(el => el.Id == hardware.Id))
					throw new ArgumentException("hardware do not contains in DB");

				_container.HardwareSet.Remove(_container.HardwareSet.Attach(hardware));
				return;
			}

			if (entity is PC)
			{
				var pc = entity as PC;

				if (!_container.PCSet.Any(el => el.Id == pc.Id))
					throw new ArgumentException("pc do not contains in DB");

				_container.PCSet.Remove(_container.PCSet.Attach(pc));
				return;
			}

			if (entity is User)
			{
				var user = entity as User;

				if (!_container.UserSet.Any(el => el.Id == user.Id))
					throw new ArgumentException("user do not contains in DB");

				_container.UserSet.Remove(_container.UserSet.Attach(user));
				return;
			}

			if (entity is UserAccess)
			{
				var userAccess = entity as UserAccess;

				if (!_container.UserAccessSet.Any(el => el.AccessType == userAccess.AccessType))
					throw new ArgumentException("userAccess do not contains in DB");

				_container.UserAccessSet.Remove(_container.UserAccessSet.Attach(userAccess));
			}

			throw new StrongTypingException(string.Format("{0} is not correct type", typeof(T)));
		}

		public T AttachEntity<T>(T entity) where T : class
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			if (entity is AccessLog)
				return _container.AccessLogSet.Attach(entity as AccessLog) as T;

			if (entity is Administrator)
				return _container.AdministratorSet.Attach(entity as Administrator) as T;

			if (entity is Document)
				return _container.DocumentSet.Attach(entity as Document) as T;

			if (entity is DocumentVersionChange)
				return _container.DocumentVersionChangeSet.Attach(entity as DocumentVersionChange) as T;

			if (entity is DocumentPath)
				return _container.DocumentPathSet.Attach(entity as DocumentPath) as T;

			if (entity is Hardware)
				return _container.HardwareSet.Attach(entity as Hardware) as T;

			if (entity is PC)
				return _container.PCSet.Attach(entity as PC) as T;

			if (entity is User)
				return _container.UserSet.Attach(entity as User) as T;

			if (entity is UserAccess)
				return _container.UserAccessSet.Attach(entity as UserAccess) as T;

			throw new StrongTypingException(string.Format("{0} is not correct type", typeof(T)));
		} 

		public T GetEntityByID<T>(int entityID) where T : class
		{
			if (typeof (T) == typeof (AccessLog))
				return _container.AccessLogSet.FirstOrDefault(el => el.Id == entityID) as T;

			if (typeof (T) == typeof (Administrator))
				return _container.AdministratorSet.FirstOrDefault(el => el.Id == entityID) as T;

			if (typeof (T) == typeof (Document))
				return _container.DocumentSet.FirstOrDefault(el => el.Id == entityID) as T;

			if (typeof (T) == typeof (DocumentPath))
				return _container.DocumentPathSet.FirstOrDefault(el => el.Id == entityID) as T;

			if (typeof(T) == typeof(DocumentVersionChange))
				return _container.DocumentVersionChangeSet.FirstOrDefault(el => el.Id == entityID) as T;

			if (typeof (T) == typeof (Hardware))
				return _container.HardwareSet.FirstOrDefault(el => el.Id == entityID) as T;

			if (typeof (T) == typeof (PC))
				return _container.PCSet.FirstOrDefault(el => el.Id == entityID) as T;

			if (typeof (T) == typeof (User))
				return _container.UserSet.FirstOrDefault(el => el.Id == entityID) as T;

			if (typeof (T) == typeof (UserAccess))
				return _container.UserAccessSet.FirstOrDefault(el => (int) el.AccessType == entityID) as T;

			throw new StrongTypingException(string.Format("{0} is not correct type", typeof(T)));
		}

		public IEnumerable<T> GetAllEntities<T>() where T : class
		{
			if (typeof (T) == typeof (AccessLog))
				return _container.AccessLogSet.Cast<T>();

			if (typeof (T) == typeof (Administrator))
				return _container.AdministratorSet.Cast<T>();

			if (typeof (T) == typeof (Document))
				return _container.DocumentSet.Cast<T>();

			if (typeof(T) == typeof(DocumentVersionChange))
				return _container.DocumentVersionChangeSet.Cast<T>();

			if (typeof (T) == typeof (DocumentPath))
				return _container.DocumentPathSet.Cast<T>();

			if (typeof (T) == typeof (Hardware))
				return _container.HardwareSet.Cast<T>();

			if (typeof (T) == typeof (PC))
				return _container.PCSet.Cast<T>();

			if (typeof (T) == typeof (User))
				return _container.UserSet.Cast<T>();

			if (typeof (T) == typeof (UserAccess))
				return _container.UserAccessSet.Cast<T>();

			throw new StrongTypingException(string.Format("{0} is not correct type", typeof(T)));
		}

		public long GetEntityCount<T>() where T : class
		{
			if (typeof (T) == typeof (AccessLog))
				return _container.AccessLogSet.LongCount();

			if (typeof (T) == typeof (Administrator))
				return _container.AdministratorSet.LongCount();

			if (typeof (T) == typeof (Document))
				return _container.DocumentSet.LongCount();

			if (typeof(T) == typeof(DocumentVersionChange))
				return _container.DocumentVersionChangeSet.LongCount();

			if (typeof (T) == typeof (DocumentPath))
				return _container.DocumentPathSet.LongCount();

			if (typeof (T) == typeof (Hardware))
				return _container.HardwareSet.LongCount();

			if (typeof (T) == typeof (PC))
				return _container.PCSet.LongCount();

			if (typeof (T) == typeof (User))
				return _container.UserSet.LongCount();

			if (typeof (T) == typeof (UserAccess))
				return _container.UserAccessSet.LongCount();

			throw new StrongTypingException(string.Format("{0} is not correct type", typeof(T)));
		}
	}
}
