using System;
using System.Data.EntityClient;
using System.Data.SqlClient;

namespace DRDLPSystemDAL
{
	internal static class ConnectionHandling
	{
		internal static EntityConnectionStringBuilder GetSpecificEntityConnectionStringBuilder(string dataSource, string initialCatalogue, string userID, 
																							 string password, bool useWindoesAutentification = false)
		{
			if (string.IsNullOrEmpty(dataSource))
				throw new ArgumentException("SQLServerName can`t be empty or null");

			if (!useWindoesAutentification && (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(password)))
				throw new ArgumentException("login or password can`t be empty or null");

			return new EntityConnectionStringBuilder
				{
					Metadata = "res://*/DRDLPModel.csdl|res://*/DRDLPModel.ssdl|res://*/DRDLPModel.msl",
					Provider = "System.Data.SqlClient",
					ProviderConnectionString = new SqlConnectionStringBuilder
													{
														DataSource = dataSource,
														InitialCatalog = initialCatalogue,
														UserID = userID,
														Password = password,
														IntegratedSecurity = useWindoesAutentification,
														MultipleActiveResultSets = true,
														ApplicationName = "EntityFramework"
													}.ToString()
				};
		}
	}
}
