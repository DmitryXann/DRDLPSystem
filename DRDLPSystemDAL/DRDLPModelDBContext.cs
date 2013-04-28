using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DRDLPSystemDAL
{
    public partial class DRDLPModelDBContext : IDisposable
    {
        private readonly DRDLPModelContainer _container;

		#region Constructors
		public DRDLPModelDBContext()
        {
            _container = new DRDLPModelContainer();
	        _container.Database.CreateIfNotExists();
        }

		public DRDLPModelDBContext(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
				throw new ArgumentException("connectionString can`t be empty or null");

			_container = new DRDLPModelContainer(connectionString);
			_container.Database.CreateIfNotExists();
		}

		public DRDLPModelDBContext(string dataSource, string initialCatalogue, string userID,
								   string password, bool useWindoesAutentification = false)
		{
			if (string.IsNullOrEmpty(dataSource))
				throw new ArgumentException("SQLServerName can`t be empty or null");

			if (!useWindoesAutentification && (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(password)))
				throw new ArgumentException("login or password can`t be empty or null");

			_container = new DRDLPModelContainer(dataSource, initialCatalogue, userID, password, useWindoesAutentification);
			_container.Database.CreateIfNotExists();
		}
		#endregion

		#region HelperMethods
		public static string GetSha512Sum(string inputData)
        {
            if (string.IsNullOrEmpty(inputData))
                throw new ArgumentException("inputData can`t be mepty or null");

            using (var sha256 = SHA512.Create())
            {
                return sha256.ComputeHash(Encoding.ASCII.GetBytes(inputData))
                             .Select(el => el.ToString("x2"))
                             .Aggregate((curEl, nextEl) => curEl + nextEl);
            }
        }

		public bool TestDBConnection(out Exception thrownException)
		{
			try
			{
				_container.AdministratorSet.LongCount();
				thrownException = null;
				return true;
			}
			catch (EntityException ex)
			{
				thrownException = new EntityException("Can`t open DB connection, check DB DataSource or user data", ex);
				return false;
			}
			catch (Exception ex)
			{
				thrownException = ex;
				return false;
			}
		}

        public void SaveChanges()
        {
            _container.SaveChanges();
        }

        public void Dispose()
        {
            _container.Dispose();
        }
        #endregion
    }
}
