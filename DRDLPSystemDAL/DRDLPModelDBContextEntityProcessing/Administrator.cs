using System;
using System.Collections.Generic;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		/// <summary>
		/// Use to add new administrator
		/// </summary>
		/// <param name="login">administrator login</param>
		/// <param name="password">administrator password, it stores in db in sha512 sum</param>
		public void AddAdministrator(string login, string password)
		{
			if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
				throw new ArgumentException("login and password cant be empty or null");

			if (_container.AdministratorSet.Any(el => el.Login.ToLower() == login.ToLower().Trim()))
				throw new ArgumentException("Administrator with this login already exists");

			_container.AdministratorSet.Add(new Administrator { Login = login.ToLower().Trim(), Password = GetSha512Sum(password.ToLower().Trim()) });
		}

		public void RemoveAdministrator(string login, string password)
		{
			if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
				throw new ArgumentException("login and password cant be empty or null");

			var selectedAdministrator = _container.AdministratorSet.FirstOrDefault(el =>
																					(el.Login.ToLower().Trim() == login.ToLower().Trim()) &&
																					(el.Password.ToLower().Trim() == password.ToLower().Trim()));
			if (selectedAdministrator == null)
				throw new ArgumentException("Administrator with this login and password do not exists");

			_container.AdministratorSet.Remove(selectedAdministrator);
		}

		public void ChangeAdministratorLoginAndOrPassword(Administrator administrator, string newLogin, string newPassword)
		{
			if (administrator == null)
				throw new ArgumentNullException("administrator");

			ChangeAdministratorLoginAndOrPassword(administrator.Login, administrator.Password, newLogin, newPassword);
		}

		public void ChangeAdministratorLoginAndOrPassword(string login, string password, string newLogin, string newPassword)
		{
			if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
				throw new ArgumentException("login and password can`t be empty or null");

			if (string.IsNullOrEmpty(newLogin) || string.IsNullOrEmpty(newPassword))
				throw new ArgumentException("newLogin and newPassword can`t be empty or null in the same time");

			var selectedAdministrator = _container.AdministratorSet.FirstOrDefault(el =>
																					(el.Login.ToLower() == login.ToLower().Trim()) &&
																					(el.Password.ToLower() == password.ToLower().Trim()));
			if (selectedAdministrator == null)
				throw new ArgumentException("Administrator with this login and password do not exists");


			if (!string.IsNullOrEmpty(newLogin))
				selectedAdministrator.Login = newLogin.ToLower().Trim();

			if (!string.IsNullOrEmpty(newPassword))
				selectedAdministrator.Password = GetSha512Sum(newPassword.ToLower().Trim());
		}

		public string ResetAdministratorsPassword(string login)
		{
			if (string.IsNullOrEmpty(login))
				throw new ArgumentException("login can`t be empty or null");

			var selectedAdministrator = _container.AdministratorSet.FirstOrDefault(el => el.Login.ToLower().Trim() == login.ToLower().Trim());

			if (selectedAdministrator == null)
				throw new ArgumentException("Administrator with this login do not exists");

			selectedAdministrator.Password = GetSha512Sum(Guid.NewGuid().ToString().Trim());
			selectedAdministrator.NeedsToChangePassword = true;

			return selectedAdministrator.Password;
		}

		public Administrator GetAdministratorByLogin(string login)
		{
			if (string.IsNullOrEmpty(login))
				throw new ArgumentException("login can`t be empty or null");

			return _container.AdministratorSet.FirstOrDefault(el => el.Login.ToLower().Trim() == login.ToLower().Trim());
		}

		public Administrator GetAdministratorByLoginAndPassword(string login, string password)
		{
			if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
				throw new ArgumentException("login and password can`t be empty or null");

			var encryptedPassword = GetSha512Sum(password.ToLower().Trim());

			return _container.AdministratorSet.FirstOrDefault(el =>
															   (el.Login.ToLower().Trim() == login.ToLower().Trim()) &&
															   (el.Password.ToLower().Trim() == encryptedPassword.Trim()));
		}

		public bool IsAdministratorWithLoginExists(string login)
		{
			if (string.IsNullOrEmpty(login))
				throw new ArgumentException("login can`t be empty or null");

			return _container.AdministratorSet.Any(el => el.Login.ToLower().Trim() == login.ToLower().Trim());
		}

		public IEnumerable<Administrator> GetAllAdministratorsWithNeedToChangePassword()
		{
			return _container.AdministratorSet.Where(el => el.NeedsToChangePassword).ToArray();
		} 
	}
}
