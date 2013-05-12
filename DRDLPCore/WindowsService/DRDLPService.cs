using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Reflection;
using System.Threading;

namespace DRDLPCore.WindowsService
{
	public partial class DRDLPService : ServiceBase
	{
		private const string WATCH_DOG_FILE_NAME = "DRDLPClipboardWatchDog.exe";
		private string _watchDogLocation;
		private string _currentWorkingDirectory;

		private delegate void WatchDogProcessTerminated();
		private event WatchDogProcessTerminated OnWatchDogProcessTerminated;

		protected DRDLPService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			_currentWorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			_watchDogLocation = _currentWorkingDirectory  + Path.DirectorySeparatorChar + WATCH_DOG_FILE_NAME;

			OnWatchDogProcessTerminated += StartWatchDog;

			var watchDogProcessWathcer = new Thread(StartWatchDog);
			watchDogProcessWathcer.Start();
		}

		private void StartWatchDog()
		{
			var watchDogProcess = new Process
			{
				StartInfo =
				{
					FileName = _watchDogLocation,
					Domain = Environment.MachineName,
					UseShellExecute = false,
					WorkingDirectory = _currentWorkingDirectory
				}
			};

			watchDogProcess.Start();
			watchDogProcess.WaitForExit();

			if (OnWatchDogProcessTerminated != null)
				OnWatchDogProcessTerminated();
		}

		protected override void OnStop()
		{
			OnWatchDogProcessTerminated -= StartWatchDog;
		}
	}
}
