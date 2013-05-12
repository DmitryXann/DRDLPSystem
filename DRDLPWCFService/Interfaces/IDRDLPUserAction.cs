using System;
using System.ServiceModel;

namespace DRDLPWCFService.Interfaces
{
	[ServiceContract]
	public interface IDRDLPUserAction
	{
		[OperationContract]
		void AddNewDocument(string documentPath, string documentPart);

		[OperationContract]
		string TryOpenDocument(Guid documentID, string documentPath);

		[OperationContract]
		void DocumentChanged(Guid documentID, long docuementVersion, string documentPart, string documentPath);
	}
}
