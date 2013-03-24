using System.ServiceProcess;

namespace DRDLPNet4_5.WindowsAdministation
{
	public static class SecondLoginServiceManagement
	{
		private const string SECOND_LOGIN_SERVICE_NAME = "seclogon";

		public static bool IsSecondLoginServiceRunning { get { return ServiceHelper.ServiceStatus(SECOND_LOGIN_SERVICE_NAME) == ServiceControllerStatus.Running; } }

		public static void StartSecondLoginService()
		{
			if (IsSecondLoginServiceRunning)
				return;

			ServiceHelper.ChangeServiceStartMode(SECOND_LOGIN_SERVICE_NAME, ServiceStartMode.Automatic);
			ServiceHelper.StartService(SECOND_LOGIN_SERVICE_NAME);
		}
	}
}
