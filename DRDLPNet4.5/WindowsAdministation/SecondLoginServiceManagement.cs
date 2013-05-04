using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace DRDLPCore.WindowsAdministation
{
	public static class SecondLoginServiceManagement
	{
		private const string SECOND_LOGIN_SERVICE_NAME = "seclogon";

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern Boolean ChangeServiceConfig(IntPtr hService,
														 uint nServiceType,
														 uint nStartType,
														 uint nErrorControl,
														 String lpBinaryPathName,
														 String lpLoadOrderGroup,
														 IntPtr lpdwTagId,
														 [In] char[] lpDependencies,
														 String lpServiceStartName,
														 String lpPassword,
														 String lpDisplayName);

		[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, WinServiceFlags dwDesiredAccess);

		[DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

		[Flags]
		private enum WinServiceFlags : uint
		{
			SERVICE_NO_CHANGE	  = 0xFFFFFFFF,
			SERVICE_QUERY_CONFIG  = 0x00000001,
			SERVICE_CHANGE_CONFIG = 0x00000002,
			SC_MANAGER_ALL_ACCESS = 0x000F003F,
		}

		public static bool IsSecondLoginServiceRunning { get { return ServiceStatus(SECOND_LOGIN_SERVICE_NAME) == ServiceControllerStatus.Running; } }

		private static void ChangeServiceStartMode(string serviceName, ServiceStartMode selectedMode)
		{
			using (var serviceController = new ServiceController(serviceName, Environment.MachineName))
			{
				ChangeServiceStartMode(serviceController, selectedMode);
			}
		}
		private static void ChangeServiceStartMode(ServiceController serviceController, ServiceStartMode selectedMode)
		{
			var serviceControllerManagerHandle = OpenSCManager(null, null, (uint)WinServiceFlags.SC_MANAGER_ALL_ACCESS);

			if (serviceControllerManagerHandle == IntPtr.Zero)
				throw new ExternalException("Open Service Manager Error");

			var serviceHandle = OpenService(
				serviceControllerManagerHandle,
				serviceController.ServiceName,
				WinServiceFlags.SERVICE_QUERY_CONFIG | WinServiceFlags.SERVICE_CHANGE_CONFIG);

			if (serviceHandle == IntPtr.Zero)
				throw new ExternalException("Open Service Error");

			var result = ChangeServiceConfig(serviceHandle,
											 (uint)WinServiceFlags.SERVICE_NO_CHANGE,
											 (uint)selectedMode,
											 (uint)WinServiceFlags.SERVICE_NO_CHANGE,
											 null,
											 null,
											 IntPtr.Zero,
											 null,
											 null,
											 null,
											 null);

			if (result)
				return;

			var win32Exception = new Win32Exception(Marshal.GetLastWin32Error());
			throw new ExternalException(string.Format("Could not change service start type: {0}", win32Exception.Message));
		}
		private static void StartService(string serviceName)
		{
			using (var secondLoginService = new ServiceController(serviceName, Environment.MachineName))
			{
				try
				{
					switch (secondLoginService.Status)
					{

						case ServiceControllerStatus.ContinuePending:
							secondLoginService.WaitForStatus(ServiceControllerStatus.Running);
							break;

						case ServiceControllerStatus.Paused:
							secondLoginService.Continue();
							break;

						case ServiceControllerStatus.PausePending:
							secondLoginService.WaitForStatus(ServiceControllerStatus.Paused);
							secondLoginService.Continue();
							break;

						case ServiceControllerStatus.Running:
							return;

						case ServiceControllerStatus.StartPending:
							secondLoginService.WaitForStatus(ServiceControllerStatus.Running);
							break;

						case ServiceControllerStatus.Stopped:
							secondLoginService.Start();
							break;

						case ServiceControllerStatus.StopPending:
							secondLoginService.WaitForStatus(ServiceControllerStatus.Stopped);
							secondLoginService.Start();
							break;
					}
				}
				catch (Exception)
				{
					throw;
				}
			}
		}
		private static ServiceControllerStatus ServiceStatus(string serviceName)
		{
			using (var secondLoginService = new ServiceController(serviceName, Environment.MachineName))
			{
				return secondLoginService.Status;
			}
		}

		public static void StartSecondLoginService()
		{
			if (IsSecondLoginServiceRunning)
				return;

			ChangeServiceStartMode(SECOND_LOGIN_SERVICE_NAME, ServiceStartMode.Automatic);
			StartService(SECOND_LOGIN_SERVICE_NAME);
		}
	}
}
