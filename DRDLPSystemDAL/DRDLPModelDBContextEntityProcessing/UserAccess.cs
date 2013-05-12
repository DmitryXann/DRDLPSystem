using System;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddUserAccess(UserAccessAccessTypeEnum userAccessType)
		{
			if (_container.UserAccessSet.Any(el => el.AccessType == userAccessType))
				throw new ArgumentException(string.Format("{0} already exists in DB", userAccessType));

			_container.UserAccessSet.Add(new UserAccess{ AccessType = userAccessType});
		}

		public void RemoveUserAccess(UserAccessAccessTypeEnum userAccessType)
		{
			var selectedUserAccess = _container.UserAccessSet.FirstOrDefault(el => el.AccessType == userAccessType);

			if (selectedUserAccess == null)
				throw new ArgumentException(string.Format("{0} not found in DB", userAccessType));

			_container.UserAccessSet.Remove(_container.UserAccessSet.Attach(selectedUserAccess));
		}

		public UserAccess GetUserAccess(UserAccessAccessTypeEnum userAccessAccessType)
		{
			return _container.UserAccessSet.FirstOrDefault(el => el.AccessType == userAccessAccessType);
		}
	}
}
