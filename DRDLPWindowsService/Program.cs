using System.ServiceProcess;

namespace DRDLPWindowsService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
            { 
                new DRDLPService() 
            };
			ServiceBase.Run(ServicesToRun);
		}
	}
}
