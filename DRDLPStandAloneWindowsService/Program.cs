using System.ServiceProcess;

namespace DRDLPStandAloneWindowsService
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
                new StandAloneService() 
            };
			ServiceBase.Run(ServicesToRun);
		}
	}
}
