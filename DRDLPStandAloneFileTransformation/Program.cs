using System;
using System.Linq;
using System.IO;

namespace DRDLPStandAloneFileTransformation
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (!args.Any() || !File.Exists(args[0]))
				return;

			var clientGUI = new StandAloneClientGUI(args[0]);
			clientGUI.OpenFile();
		}
	}
}
