using System;

namespace DRDLPWCFService.Helpers
{
	internal static class DBAuthorization
	{
		internal struct DBConnectionData
		{
			internal string DataSource;
			internal string InitialCatalogue;
			internal string UserID;
			internal string Password;

			internal DBConnectionData(string dataSource, string initialCatalogue, string userID, string password)
			{
				DataSource = dataSource;
				InitialCatalogue = initialCatalogue;
				UserID = userID;
				Password = password;
			}
		}

		internal static void SaveDBAuthorizationData(DBConnectionData dbConnectionData)
		{
			throw new NotImplementedException();
		}

		internal static DBConnectionData GetDBAuthorizationData()
		{
			return new DBConnectionData("192.168.1.102", "TestDB", "Admin", "123");//TODO: do not forget to implement!!!!
		}
	}
}
