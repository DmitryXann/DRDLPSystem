using System;
using System.Collections.Generic;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddAccessLog(AccessLog accessLog)
		{
			if (accessLog == null)
				throw new ArgumentNullException("accessLog");

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
		}

		public void AddAccessLog(DocumentPath documentPath, Document document, Hardware hardware, PC pc, User user, 
			UserAccess userAccess, DateTime dateTime, AccessLogAccessType accessType, bool logEntryProcessed = false)
		{
			if (documentPath == null)
				throw new ArgumentNullException("documentPath");

			if (document == null)
				throw new ArgumentNullException("document");

			if (hardware == null)
				throw new ArgumentNullException("hardware");

			if (pc == null)
				throw new ArgumentNullException("pc");

			if (user == null)
				throw new ArgumentNullException("user");

			if (userAccess == null)
				throw new ArgumentNullException("userAccess");

			if (!_container.DocumentSet.Contains(document))
				throw new ArgumentException("document do not contains in DB");

			if (!_container.DocumentPathSet.Contains(documentPath))
				throw new ArgumentException("documentPath do not contains in DB");

			if (!_container.HardwareSet.Contains(hardware))
				throw new ArgumentException("hardware do not contains in DB");

			if (!_container.PCSet.Contains(pc))
				throw new ArgumentException("pc do not contains in DB");

			if (!_container.UserSet.Contains(user))
				throw new ArgumentException("user do not contains in DB");

			if (!_container.UserAccessSet.Contains(userAccess))
				throw new ArgumentException("userAccess do not contains in DB");


			_container.AccessLogSet.Add(new AccessLog
				{
					AccessDateTime = dateTime == DateTime.MinValue ? DateTime.Now : dateTime,
					AccessType = accessType,
					Document = _container.DocumentSet.Attach(document),
					DocumentPath = _container.DocumentPathSet.Attach(documentPath),
					Hardware = _container.HardwareSet.Attach(hardware),
					PC = _container.PCSet.Attach(pc),
					User = _container.UserSet.Attach(user),
					UserAccess = _container.UserAccessSet.Attach(userAccess),
					LogEntryProcessed = logEntryProcessed
				});
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


		public AccessLog GetAccessLogByID(int accessLogId)
		{
			return _container.AccessLogSet.FirstOrDefault(el => el.Id == accessLogId);
		}

		public IEnumerable<AccessLog> GetAllAccessLogs()
		{
			return _container.AccessLogSet.ToArray();
		}  

		public IEnumerable<AccessLog> GetAllAccessLogByType(AccessLogAccessType type)
		{
			return _container.AccessLogSet.Where(el => el.AccessType == type).ToArray();
		} 

		public IEnumerable<AccessLog> GetAllNotProcessedLogEntries()
		{
			return _container.AccessLogSet.Where(el => !el.LogEntryProcessed).ToArray();
		}

		public long GetAccessLogCount()
		{
			return _container.AccessLogSet.LongCount();
		}
	}
}
