using System;
using DRDLPSystemDAL;

namespace DRDLPWCFService.Helpers
{
	internal static class DBConnectionProvider
	{
		internal static DRDLPModelDBContext CreateConnection()
		{
			var connectionData = DBAuthorization.GetDBAuthorizationData();

			if (connectionData.IsDefault())
			{
				//TODO log incorrect connection
				return null;
			}

			var dbContext = new DRDLPModelDBContext(connectionData.DataSource, connectionData.InitialCatalogue, 
				connectionData.UserID, connectionData.Password);

			Exception thrownException;

			if (dbContext.TestDBConnection(out thrownException))
				return dbContext;

			dbContext.Dispose();
			//TODO: log exception
			return null;
		}
	}
}
