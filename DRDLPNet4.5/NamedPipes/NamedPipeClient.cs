using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;

namespace DRDLPNet4_5.NamedPipes
{
	public class NamedPipeClientClipboardHandler : IDisposable
	{
		private const string SERVER_NAME = ".";
		private readonly NamedPipeClientStream _namedPipeClientStream;
		private readonly StreamWriter _streamWriter;
		//private readonly StreamReader _streamReader;

		public NamedPipeClientClipboardHandler()
		{
			using (var namingPipeClient = new NamedPipeClientStream(SERVER_NAME, NamedPipesSharedData.LisenerServerPipeName))
			{
				namingPipeClient.Connect();
				var pipeWriter = new StreamWriter(namingPipeClient);
				var pipeReader = new StreamReader(namingPipeClient);

				pipeWriter.WriteLine(NamedPipesSharedData.LisenerServerPipeName);
				pipeWriter.Flush();
				namingPipeClient.WaitForPipeDrain();

				var readedName = pipeReader.ReadLine();

				if (string.IsNullOrEmpty(readedName))
					return;

				_namedPipeClientStream = new NamedPipeClientStream(SERVER_NAME, readedName, PipeDirection.InOut);
				_streamWriter = new StreamWriter(_namedPipeClientStream);
				_namedPipeClientStream.Connect();
				//_streamReader = new StreamReader(_namedPipeClientStream);
			}
		}

		private void SendInfoToServer(int pid, NamedPipesSharedData.NameedPipesServerAction action)
		{
			_streamWriter.WriteLine(pid);
			_streamWriter.WriteLine(action);
			_streamWriter.Flush();
			_namedPipeClientStream.WaitForPipeDrain();
		}
		public bool AddPIDToWatchList(int pid)
		{
			if (!Process.GetProcesses().Any(el => el.Id == pid))
				return false;

			SendInfoToServer(pid, NamedPipesSharedData.NameedPipesServerAction.AddPID);
			return true;
		}

		public bool RemovePIDFromWathList(int pid)
		{
			if (!Process.GetProcesses().Any(el => el.Id == pid))
				return false;

			SendInfoToServer(pid, NamedPipesSharedData.NameedPipesServerAction.RemovePID);
			return true;
		}

		public void Dispose()
		{
			if (_namedPipeClientStream != null)
				_namedPipeClientStream.Dispose();
		}
	}
}
