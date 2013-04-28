using System;
using System.Collections.Generic;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddUser(User user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			if (user.PC == null)
				throw new ArgumentNullException("user.PC");

			if (_container.UserSet.Contains(user))
				throw new ArgumentException("DB already contains this user");

			_container.UserSet.Add(user);
		}

		public void AddUser(PC userPC, string login, bool valid = true)
		{
			if (userPC == null)
				throw new ArgumentNullException("userPC");

			if (!_container.PCSet.Contains(userPC))
				throw new ArgumentException("No such pc found in DB");
			
			if (string.IsNullOrEmpty(login))
				throw new ArgumentException("login can`t be empty or null");

			var currentUserPC = _container.PCSet.Attach(userPC);
			var user = new User {Login = login.ToLower().Trim(), Valid = valid, PC = currentUserPC};

			_container.UserSet.Add(user);
			currentUserPC.Users.Add(user);
		}

		public User AttachUser(User user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			return _container.UserSet.Attach(user);
		}

		public void RemoveUser(User user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			_container.UserSet.Remove(_container.UserSet.Attach(user));
		}

		public void ChangeUserValidation(User user, bool isUserValid)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			if (!_container.UserSet.Contains(user))
				throw new ArgumentException("No such user found in DB");

			_container.UserSet.Attach(user).Valid = isUserValid;
		}

		public User GetUserByID(int userID)
		{
			return _container.UserSet.FirstOrDefault(el => el.Id == userID);
		}

		public IEnumerable<User> GetAllUsers()
		{
			return _container.UserSet.ToArray();
		}

		public IEnumerable<User> GetAllValidUsers()
		{
			return _container.UserSet.Where(el => el.Valid).ToArray();
		} 

		public IEnumerable<User> GetAllNotValidUsers()
		{
			return _container.UserSet.Where(el => !el.Valid).ToArray();
		} 

		public long GetUserCount()
		{
			return _container.UserSet.LongCount();
		}
	}
}
