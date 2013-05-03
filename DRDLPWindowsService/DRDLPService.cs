using System;
using System.Diagnostics;
using System.ServiceProcess;
using DRDLPNet4_5.WindowsAdministation;

namespace DRDLPWindowsService
{
	public partial class DRDLPService : ServiceBase
	{
		private ServiceNamedPipeServer _serverPipe;

		public DRDLPService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			throw new NotImplementedException();
#if DEBUG
			Debugger.Launch();
#endif
			_serverPipe = new ServiceNamedPipeServer();
			_serverPipe.StartPipeServer();
		}

		protected override void OnStop()
		{
			_serverPipe.StopPipeServer(10);
		}
	}
}
