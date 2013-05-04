using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace DRDLPCore.NamedPipes
{
	public abstract class NamedPipeServer
	{
		protected virtual List<Thread> StartedThreads { get; set; }
		protected virtual bool ServerIsActive { get; set; }

		public virtual void StartPipeServer()
		{
			ServerIsActive = true;
			StartedThreads = new List<Thread>();

			var lisenerThread = new Thread(arg =>
				{
					using (
						var pipeServerSteam = new NamedPipeServerStream(NamedPipesSharedData.LisenerServerPipeName, PipeDirection.InOut))
					{
						while (ServerIsActive)
						{
							pipeServerSteam.WaitForConnection();

							var pipeReader = new StreamReader(pipeServerSteam);
							var pipeClientRequest = pipeReader.ReadLine();

							if (string.IsNullOrEmpty(pipeClientRequest) ||
							    !pipeClientRequest.Equals(NamedPipesSharedData.LisenerServerPipeName))
							{
								pipeServerSteam.Disconnect();
								continue;
							}

							var pipeWriter = new StreamWriter(pipeServerSteam);
							var selectedPipeName = DataCryptography.GetHashSum(SystemInformation.GetRandomSysInfo + DateTime.Now.Ticks, DataCryptography.HashSum.Md5);

							pipeWriter.WriteLine(selectedPipeName);
							pipeWriter.Flush();
							pipeServerSteam.WaitForPipeDrain();

							var newConversationThread = new Thread(ConversationThread) { Name = string.Format("DRDLPConv.PipeName:{0}", selectedPipeName) };
							newConversationThread.Start(selectedPipeName);

							StartedThreads.Add(newConversationThread);

							pipeServerSteam.Disconnect();
						}
					}
				})
				{
					Name = "DRDLPPipeListener"
				};

			lisenerThread.Start();
			StartedThreads.Add(lisenerThread);
		}

		public virtual void StopPipeServer()
		{
			ServerIsActive = false;
		}

		protected abstract void ConversationThread(object conversationThreadPipeName);
	}
}
