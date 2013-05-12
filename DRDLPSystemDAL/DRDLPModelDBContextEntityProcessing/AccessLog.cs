using System;
using System.Collections.Generic;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddAccessLog(DocumentPath documentPath, Document document, IEnumerable<Hardware> hardwareCollection, PC pc, User user, 
			UserAccess userAccess, DateTime dateTime, AccessLogAccessType accessType, bool logEntryProcessed = false)
		{
			if (documentPath == null)
				throw new ArgumentNullException("documentPath");

			if (document == null)
				throw new ArgumentNullException("document");

			if (hardwareCollection == null)
				throw new ArgumentNullException("hardware");

			if (pc == null)
				throw new ArgumentNullException("pc");

			if (user == null)
				throw new ArgumentNullException("user");

			if (userAccess == null)
				throw new ArgumentNullException("userAccess");

			if (!_container.DocumentSet.Any(el => el.Id == document.Id))
				throw new ArgumentException("document do not contains in DB");

			if (!_container.DocumentPathSet.Any(el => el.Id == documentPath.Id))
				throw new ArgumentException("documentPath do not contains in DB");

			if (!_container.HardwareSet.Any(el => hardwareCollection.Any(elem => elem.Id == el.Id)))
				throw new ArgumentException("all hardware do not contains in DB");

			if (!_container.PCSet.Any(el => el.Id == pc.Id))
				throw new ArgumentException("pc do not contains in DB");

			if (!_container.UserSet.Any(el => el.Id == user.Id))
				throw new ArgumentException("user do not contains in DB");

			if (!_container.UserAccessSet.Any(el => el.AccessType == userAccess.AccessType))
				throw new ArgumentException("userAccess do not contains in DB");
			var accessLog = new AccessLog
				{
					AccessDateTime = dateTime == DateTime.MinValue ? DateTime.Now : dateTime,
					AccessType = accessType,
					Document = _container.DocumentSet.Attach(document),
					DocumentPath = _container.DocumentPathSet.Attach(documentPath),
					PC = _container.PCSet.Attach(pc),
					User = _container.UserSet.Attach(user),
					UserAccess = _container.UserAccessSet.Attach(userAccess),
					LogEntryProcessed = logEntryProcessed
				};

			foreach (var hardware in hardwareCollection)
			{
				accessLog.Hardware.Add(_container.HardwareSet.Attach(hardware));
			}
		}

		public void ChangeLogEntryProcessedFlag(AccessLog accessLog, bool logEntryProcessed = true)
		{
			if (accessLog == null)
				throw new ArgumentNullException("accessLog");

			var selectedAccessLog = _container.AccessLogSet.FirstOrDefault(el => el == _container.AccessLogSet.Attach(accessLog));

			if (selectedAccessLog == null)
				throw new ArgumentException("accessLog do not contains in DB");

			selectedAccessLog.LogEntryProcessed = logEntryProcessed;
		}

		public void ChangeLogEntryProcessedFlag(int accessLogId, bool logEntryProcessed = true)
		{
			var selectedAccessLog = _container.AccessLogSet.FirstOrDefault(el => el.Id == accessLogId);

			if (selectedAccessLog == null)
				throw new ArgumentException("accessLog do not contains in DB");

			selectedAccessLog.LogEntryProcessed = logEntryProcessed;
		}

		public IEnumerable<AccessLog> GetAllAccessLogByType(AccessLogAccessType type)
		{
			return _container.AccessLogSet.Where(el => el.AccessType == type).ToArray();
		} 

		public IEnumerable<AccessLog> GetAllNotProcessedLogEntries()
		{
			return _container.AccessLogSet.Where(el => !el.LogEntryProcessed).ToArray();
		}
	}
}
