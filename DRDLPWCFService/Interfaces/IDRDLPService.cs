using System.ServiceModel;

namespace DRDLPWCFService.Interfaces
{
	[ServiceContract]
	public interface IDRDLPService
	{
		[OperationContract]
		IDRDLPUserAction EstablishUserConnection(string hardwareBasePCID, string userLogin);

		[OperationContract]
		IDRDLPAdminAction EstablishAdminConnection(string hardwareBasePCID, string adminLogin, string adminPassword);
	}
}
