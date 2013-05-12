using System.Collections.Generic;
using System.ServiceModel;
using DRDLPWCFService.Helpers;

namespace DRDLPWCFService.Interfaces
{
	[ServiceContract]
	public interface IDRDLPAdminAction
	{
		[OperationContract]
		bool AddNewPC(IEnumerable<DALHardware> hardwareInfo, IEnumerable<DALUser> users);

		[OperationContract]
		bool AddNewUser(string login);

		[OperationContract]
		bool ChangeUserLogin(string oldLogin, string newLogin);
	}
}
