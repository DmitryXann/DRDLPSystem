using System.Linq;
using DRDLPWCFService.Helpers;
using DRDLPWCFService.Interfaces;

namespace DRDLPWCFService
{
	public class DRDLPService : IDRDLPService
	{
		public IDRDLPUserAction EstablishUserConnection(string hardwareBasePCID, string userLogin)
		{
			if (string.IsNullOrEmpty(hardwareBasePCID) || string.IsNullOrEmpty(userLogin))
				return null;

			using (var context = DBConnectionProvider.CreateConnection())
			{
				if (context == null)
					return null;

				var selectedPC = context.GetPCByHardwareBasedID(hardwareBasePCID);

				if ((selectedPC == null) || !selectedPC.Valid)
					return null;

				var selectedUser = selectedPC.Users.FirstOrDefault(el => el.Login.ToLower().Trim() == userLogin.ToLower().Trim());

				return selectedUser == null 
									? null 
									: (selectedUser.Valid 
													? new DRDLPUserAction(selectedPC.Id, selectedUser.Id)
													: null);
			}
		}

		public IDRDLPAdminAction EstablishAdminConnection(string hardwareBasePCID, string adminLogin, string adminPassword)
		{
			if (string.IsNullOrEmpty(hardwareBasePCID) || string.IsNullOrEmpty(adminLogin) || string.IsNullOrEmpty(adminPassword))
				return null;

			using (var context = DBConnectionProvider.CreateConnection())
			{
				if (context == null)
					return null;

				var selectedAdmin = context.GetAdministratorByLoginAndPassword(adminLogin.ToLower().Trim(), adminPassword.ToLower().Trim());

				return selectedAdmin == null 
										? null 
										: (selectedAdmin.NeedsToChangePassword
													? null
													: new DRDLPAdminAction(hardwareBasePCID, selectedAdmin.Id));
			}
		}
	}
}
