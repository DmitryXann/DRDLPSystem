using System;
using System.Collections.Generic;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddUser(PC userPC, string login, bool valid = true)
		{
			if (userPC == null)
				throw new ArgumentNullException("userPC");

			if (!_container.PCSet.Any(el => el.Id == userPC.Id))
				throw new ArgumentException("No such pc found in DB");
			
			if (string.IsNullOrEmpty(login))
				throw new ArgumentException("login can`t be empty or null");

			var currentUserPC = _container.PCSet.Attach(userPC);
			var user = new User {Login = login.ToLower().Trim(), Valid = valid, PC = currentUserPC};

			_container.UserSet.Add(user);
			currentUserPC.Users.Add(user);
		}

		public void ChangeUserValidation(User user, bool isUserValid)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			if (!_container.UserSet.Any(el => el.Id == user.Id))
				throw new ArgumentException("No such user found in DB");

			_container.UserSet.Attach(user).Valid = isUserValid;
		}

		public bool IsUserEsists(string login)
		{
			if (string.IsNullOrEmpty(login))
				throw new ArgumentException("login can`t be empty or null");

			return _container.UserSet.Any(el => el.Login.ToLower().Trim() == login.ToLower().Trim());
		}

		public bool IsUserExistsAndValid(string login)
		{
			if (string.IsNullOrEmpty(login))
				throw new ArgumentException("login can`t be empty or null");

			return _container.UserSet.Any(el => (el.Login.ToLower().Trim() == login.ToLower().Trim()) && el.Valid);
		}

		public IEnumerable<User> GetAllValidUsers()
		{
			return _container.UserSet.Where(el => el.Valid).ToArray();
		} 

		public IEnumerable<User> GetAllNotValidUsers()
		{
			return _container.UserSet.Where(el => !el.Valid).ToArray();
		} 
	}
}
