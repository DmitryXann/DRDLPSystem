using System;
using System.Collections.Generic;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddUserAccess(UserAccess userAccess)
		{
			if (userAccess == null)
				throw new ArgumentNullException("userAccess");

			if (_container.UserAccessSet.Contains(userAccess))
				throw new ArgumentException("Selected user access type already exist in DB");

			_container.UserAccessSet.Add(userAccess);
		}

		public void AddUserAccess(UserAccessAccessTypeEnum userAccessType)
		{
			if (_container.UserAccessSet.Any(el => el.AccessType == userAccessType))
				throw new ArgumentException(string.Format("{0} already exists in DB", userAccessType));

			_container.UserAccessSet.Add(new UserAccess{ AccessType = userAccessType});
		}

		public UserAccess AttachUserAccess(UserAccess userAccess)
		{
			if (userAccess == null)
				throw new ArgumentNullException("userAccess");

			return _container.UserAccessSet.Attach(userAccess);
		}

		public void RemoveUserAccess(UserAccess userAccess)
		{
			if (userAccess == null)
				throw new ArgumentNullException("userAccess");

			_container.UserAccessSet.Remove(_container.UserAccessSet.Attach(userAccess));
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

		public IEnumerable<UserAccess> GetAllUserAccess()
		{
			return _container.UserAccessSet.ToArray();
		} 

		public int GetUserAccessCount()
		{
			return _container.UserAccessSet.Count();
		}
	}
}
