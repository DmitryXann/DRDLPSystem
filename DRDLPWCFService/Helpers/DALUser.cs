using System;
using DRDLPSystemDAL.Interfaces;

namespace DRDLPWCFService.Helpers
{
	public class DALUser : IUser
	{
		public string Login { get; set; }
		public bool Valid { get; set; }

		public DALUser(string login, bool valid)
		{
			if (string.IsNullOrEmpty(login))
				throw new ArgumentException("login can`t be empty or null");

			Login = login;
			Valid = valid;
		}
	}
}
