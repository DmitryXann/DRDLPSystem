using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace DRDLPCore.WindowsAdministation
{
	static class LocalUserAdministation
	{
		private const string DRDLP_SYSTEM_USER_GROUP_NAME = "DRDLPSystemUserGroup";
		private const string DRDLP_SYSTEM_USER_GROUP_DESCRIPTION = "This group contains DRDLPSystem user, witch is needed for security reasons";

		private const string DRDLP_SYSTEM_USER_NAME = "DRDLPSystemUser";
		private const string DRDLP_SYSTEM_USER_DESCRIPTION = "This is DRDLPSystem user, witch is needed for security reasons";

		private static readonly string USER_USER_GROUP_PATH = string.Format("WinNT://{0},computer", Environment.MachineName);
		private static readonly string USER_PASSWORD = DataCryptography.GetHashSum(SystemInformation.GetCPUSerialInfo.FirstOrDefault() +
																				   SystemInformation.GetCPUName.FirstOrDefault() +
																				   DRDLP_SYSTEM_USER_NAME, DataCryptography.HashSum.Md5);
		private enum ClassName
		{
			User,
			Group
		}

		public static bool IsDRDLPSystemUserGroupExist { get { return IsDRDLPSystemUserOrUserGroupExists(ClassName.Group, DRDLP_SYSTEM_USER_GROUP_NAME); } }
		public static bool IsDRDLPSystemUserExists { get { return IsDRDLPSystemUserOrUserGroupExists(ClassName.User, DRDLP_SYSTEM_USER_NAME); } }
		public static bool IsDRDLPSystemUserEnabled
		{
			get
			{
				using (var principalContext = new PrincipalContext(ContextType.Machine))
				{
					var selecteduser = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, DRDLP_SYSTEM_USER_NAME);
					return selecteduser.Enabled.HasValue ? selecteduser.Enabled.Value : false;
				}
			}
		}

		public static string GetDRDLPSystemUserName
		{
			get
			{
				if (IsDRDLPSystemUserExists)
					return DRDLP_SYSTEM_USER_NAME;

				return CreateNewDefaultUser() ? DRDLP_SYSTEM_USER_NAME : string.Empty;
			}
		}
		public static string GetDRDLPSystemUserPassword
		{
			get
			{
				if (IsDRDLPSystemUserExists)
					return USER_PASSWORD;

				return CreateNewDefaultUser() ? USER_PASSWORD : string.Empty;
			}
		}

		private static bool IsDRDLPSystemUserOrUserGroupExists(ClassName neededClassName, string expectedName)
		{
			using (var localComputerEntry = new DirectoryEntry(USER_USER_GROUP_PATH))
			{
				return localComputerEntry.Children.Cast<DirectoryEntry>()
										 .Any(el => (el.SchemaClassName == neededClassName.ToString()) &&
													(el.Name == expectedName));

			}
		}
		private static void CreateNewUserGroup(string userGroupName, string userGroupDescription)
		{
			using (var context = new PrincipalContext(ContextType.Machine))
			{
				var newGroup = new GroupPrincipal(context) { Name = userGroupName, Description = userGroupDescription };
				newGroup.Save();
			}
		}
		private static bool CreateNewUser(string userName, string fullUserName, string userPassword, string userDescription, string newUserGroup, string groupSourceUser = null, bool userCannotChangePass = true, bool passwordNeverExpires = true)
		{
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

					if (!string.IsNullOrEmpty(groupSourceUser))
					{
						var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.Name, groupSourceUser);

						if ((userPrincipal != null))
						{
							var groupsToAdd = userPrincipal.GetGroups();

							if (groupsToAdd.Any())
							{
								foreach (var selectedGroup in groupsToAdd.Select(el => el.SamAccountName).Select(el => GroupPrincipal.FindByIdentity(context, el)))
								{
									selectedGroup.Members.Add(newUser);
									selectedGroup.Save();
								}
							}
						}
					}

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
		private static void ActivateUserAccount(string userName)
		{
			using (var principalContext = new PrincipalContext(ContextType.Machine))
			{
				var selecteduser = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, userName);
				selecteduser.Enabled = true;

				selecteduser.Save();
			}
		}

		public static bool CreateNewDefaultUser()
		{
			if (!IsDRDLPSystemUserExists)
			{
				if (!IsDRDLPSystemUserGroupExist)
					CreateNewUserGroup(DRDLP_SYSTEM_USER_GROUP_NAME, DRDLP_SYSTEM_USER_GROUP_DESCRIPTION);

				return CreateNewUser(DRDLP_SYSTEM_USER_NAME, DRDLP_SYSTEM_USER_NAME, USER_PASSWORD, DRDLP_SYSTEM_USER_DESCRIPTION, DRDLP_SYSTEM_USER_GROUP_NAME, UserPrincipal.Current.Name);
			}

			return true;
		}
		public static void ActivateUserAccount()
		{
			ActivateUserAccount(DRDLP_SYSTEM_USER_NAME);
		}
	}
}
