namespace DRDLPSystemDAL
{
	public partial class DRDLPModelContainer
	{
		public DRDLPModelContainer(string connectionString)
			: base(connectionString)
		{
		}

		public DRDLPModelContainer(string dataSource, string initialCatalogue, string userID,
		                           string password, bool useWindoesAutentification = false)
			: base(ConnectionHandling.GetSpecificEntityConnectionStringBuilder(dataSource, initialCatalogue, userID, password, useWindoesAutentification).ToString())
		{
		}
	}
}
