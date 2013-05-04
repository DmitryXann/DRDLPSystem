using System;

namespace DRDLPClipboardWatchDog
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			var serverPipe = new WatchDogNamedPipeServer();
			serverPipe.StartPipeServer();
		}
	}
}
