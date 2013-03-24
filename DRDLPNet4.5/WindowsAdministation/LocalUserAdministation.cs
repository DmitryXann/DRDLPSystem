using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace DRDLPNet4_5.WindowsAdministation
{
	static class LocalUserAdministation
	{
		private const string DRDLP_SYSTEM_USER_GROUP_NAME = "DRDLPSystemUserGroup";
		private const string DRDLP_SYSTEM_USER_GROUP_DESCRIPTION = "This group contains DRDLPSystem user, witch is needed for security reasons";

		private const string DRDLP_SYSTEM_USER_NAME = "DRDLPSystemUser";
		private const string DRDLP_SYSTEM_USER_DESCRIPTION = "This is DRDLPSystem user, witch is needed for security reasons";

		private static readonly string USER_USER_GROUP_PATH = string.Format("WinNT://{0},computer", Environment.MachineName);

		private enum ClassName
		{
			User,
			Group
		}

		public static bool IsDRDLPSystemUserGroupExist { get { return IsDRDLPSystemUserOrUserGroupExists(ClassName.Group, DRDLP_SYSTEM_USER_GROUP_NAME); } }
		public static bool IsDRDLPSystemUserExists { get { return IsDRDLPSystemUserOrUserGroupExists(ClassName.User, DRDLP_SYSTEM_USER_NAME); } }
		public static string GetDRDLPSystemUserName
		{
			get
			{
				if (IsDRDLPSystemUserExists)
					return DRDLP_SYSTEM_USER_NAME;

				return CreateNewDefaultUser() ? DRDLP_SYSTEM_USER_NAME : string.Empty;
			}
		}

		private static bool IsDRDLPSystemUserOrUserGroupExists(ClassName neededClassName, string expectedName)
		{
			if (string.IsNullOrEmpty(expectedName))
				throw new ArgumentException("expectedName can`t be empty or null");

			using (var localComputerEntry = new DirectoryEntry(USER_USER_GROUP_PATH))
			{
				return localComputerEntry.Children.Cast<DirectoryEntry>()
										 .Any(el => (el.SchemaClassName == neededClassName.ToString()) &&
													(el.Name == expectedName));

			}
		}
		private static void CreateNewUserGroup(string userGroupName, string userGroupDescription)
		{
			if (string.IsNullOrEmpty(userGroupName))
				throw new ArgumentException("userGroupName can`t be empty or null");

			if (string.IsNullOrEmpty(userGroupDescription))
				throw new ArgumentException("userGroupDescription can`t be empty or null");

			using (var context = new PrincipalContext(ContextType.Machine))
			{
				var newGroup = new GroupPrincipal(context) { Name = userGroupName, Description = userGroupDescription };
				newGroup.Save();
			}
		}
		private static bool CreateNewUser(string userName, string fullUserName, string userPassword, string userDescription, string newUserGroup, bool userCannotChangePass = true, bool passwordNeverExpires = true)
		{
			if (string.IsNullOrEmpty(userName))
				throw new ArgumentException("userName can`t be empty or null");

			if (string.IsNullOrEmpty(userPassword))
				throw new ArgumentException("userPassword can`t be empty or null");

			if (string.IsNullOrEmpty(newUserGroup))
				throw new ArgumentException("userPassword can`t be empty or null");

			if (IsDRDLPSystemUserOrUserGroupExists(ClassName.User, userName))
				throw new Exception("User already exists");

			if (!IsDRDLPSystemUserOrUserGroupExists(ClassName.Group, newUserGroup))
				throw new Exception(string.Format("{0} group do not exists", newUserGroup));

			try
			{
				using (var context = new PrincipalContext(ContextType.Machine))
				{
					var newUser = new UserPrincipal(context)
					{
						DisplayName = fullUserName,
						Name = userName,
						Description = userDescription,
						UserCannotChangePassword = userCannotChangePass,
						PasswordNeverExpires = passwordNeverExpires
					};

					newUser.SetPassword(userPassword);
					newUser.Save();

					var userGroup = GroupPrincipal.FindByIdentity(context, newUserGroup);
					userGroup.Members.Add(newUser);
					userGroup.Save();
				}

				return true;
			}
			catch
			{
				return false;
			}
		}
		
		public static bool CreateNewDefaultUser()
		{
			if (!IsDRDLPSystemUserExists)
			{
				if (!IsDRDLPSystemUserGroupExist)
					CreateNewUserGroup(DRDLP_SYSTEM_USER_GROUP_NAME, DRDLP_SYSTEM_USER_GROUP_DESCRIPTION);

				return CreateNewUser(DRDLP_SYSTEM_USER_NAME, DRDLP_SYSTEM_USER_NAME, "0123", DRDLP_SYSTEM_USER_DESCRIPTION, DRDLP_SYSTEM_USER_GROUP_NAME);
			}

			return true;
		}	
	}
}
