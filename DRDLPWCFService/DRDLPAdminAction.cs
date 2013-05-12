using System;
using System.Linq;
using System.Collections.Generic;
using DRDLPSystemDAL;
using DRDLPWCFService.Helpers;
using DRDLPWCFService.Interfaces;

namespace DRDLPWCFService
{
	public class DRDLPAdminAction : IDRDLPAdminAction
	{
		private readonly string _hardwareID;
		private readonly int _adminID; //TODO: implement logging

		internal DRDLPAdminAction(string hardwareBasePCID, int adminID)
		{
			_hardwareID = hardwareBasePCID;
			_adminID = adminID;
		}

		public bool AddNewPC(IEnumerable<DALHardware> hardwareInfo, IEnumerable<DALUser> users)
		{
			if ((hardwareInfo == null) ||
				(hardwareInfo.Any(el => string.IsNullOrEmpty(el.HardwareID) && string.IsNullOrEmpty(el.Name))))
				return false;

			using (var context = DBConnectionProvider.CreateConnection())
			{
				if (context == null)
					return false;

				try
				{
					var newPC = new PC { PCHardwareBasedID = _hardwareID };

					foreach (var selectedHardware in hardwareInfo.Select(dalHardware => new Hardware
					{
						HardwareID = dalHardware.HardwareID,
						Name = dalHardware.Name,
						OtherInfo = dalHardware.OtherInfo,
						Type = dalHardware.Type,
						PC = newPC
					}))
					{
						newPC.Hardware.Add(selectedHardware);
						context.AddEntity(selectedHardware);
					}

					foreach (var dalUser in users.Select(el => new User { Login = el.Login, Valid = el.Valid}))
					{
						newPC.Users.Add(dalUser);
						context.AddEntity(dalUser);
					}
					context.AddEntity(newPC);
					context.SaveChanges();

					return true;
				}
				catch (Exception ex)
				{
					//TODO: log ex
					return false;
				}
			}
		}

		public bool AddNewUser(string login)
		{
			if (string.IsNullOrEmpty(login))
				return false;

			using (var context = DBConnectionProvider.CreateConnection())
			{
				if (context == null)
					return false;

				var selectedPC = context.GetPCByHardwareBasedID(_hardwareID);

				if (selectedPC == null)
					return false;

				try
				{
					var newUser = new User { Login = login.ToLower().Trim(), Valid = true, PC = selectedPC };
					context.AddEntity(newUser);
					selectedPC.Users.Add(newUser);

					context.SaveChanges();

					return true;
				}
				catch (Exception ex)
				{
					//TODO: log ex
					return false;
				}
			}
		}

		public bool ChangeUserLogin(string oldLogin, string newLogin)
		{
			if (string.IsNullOrEmpty(oldLogin) || string.IsNullOrEmpty(newLogin))
				return false;

			using (var context = DBConnectionProvider.CreateConnection())
			{
				if (context == null)
					return false;

				var selectedPC = context.GetPCByHardwareBasedID(_hardwareID);

				if ((selectedPC == null) || (selectedPC.Users == null))
					return false;

				var selectedUser = selectedPC.Users.FirstOrDefault(el => el.Login.ToLower().Trim() == oldLogin.ToLower().Trim());

				if (selectedUser == null)
					return false;

				selectedUser.Login = newLogin;

				context.SaveChanges();

				return true;
			}
		}
	}
}
