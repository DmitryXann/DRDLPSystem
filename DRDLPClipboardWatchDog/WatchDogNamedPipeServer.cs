﻿using System.IO;
using System.IO.Pipes;
using System.Threading;
using DRDLPCore.NamedPipes;

namespace DRDLPClipboardWatchDog
{
	internal class WatchDogNamedPipeServer : NamedPipeServer
	{
		private static ClipboardHandler _clipboardHandler;
		
		private void StartClipBoardHandling()
		{
			_clipboardHandler.HandleCrossAppBufferDataTransmission();
		}

		protected override void ConversationThread(object conversationThreadPipeName)
		{
			var selectedPipeName = conversationThreadPipeName as string;

			if (string.IsNullOrEmpty(selectedPipeName))
				return;

			_clipboardHandler = new ClipboardHandler();

			using (var pipeServerSteam = new NamedPipeServerStream(selectedPipeName, PipeDirection.InOut, 1))
			{
				pipeServerSteam.WaitForConnection();

				var pipeReader = new StreamReader(pipeServerSteam);
				//var pipeWriter = new StreamWriter(pipeServerSteam);

				//pipeWriter.WriteLine(selectedPipeName);
				//pipeWriter.Flush();
				//pipeServerSteam.WaitForPipeDrain();

				var selectedAction = NamedPipesSharedData.NameedPipesServerAction.AddPID;

				while ((selectedAction != NamedPipesSharedData.NameedPipesServerAction.StopClipboardHandling) && ServerIsActive)
				{
					int recivedPID;

					if (!int.TryParse(pipeReader.ReadLine(), out recivedPID) ||
						!NamedPipesSharedData.NameedPipesServerAction.TryParse(pipeReader.ReadLine(), out selectedAction))
					{
						//pipeWriter.WriteLine(NamedPipesSharedData.NameedPipesServerAction.ReceiveError);
						//pipeWriter.Flush();
						//pipeServerSteam.WaitForPipeDrain();
						continue;
					}

					switch (selectedAction)
					{
						case NamedPipesSharedData.NameedPipesServerAction.AddPID:
							if (!_clipboardHandler.PIDIsCurrentlyHandled(recivedPID))
							{
								_clipboardHandler.AddNewProcess(recivedPID);

								if (!_clipboardHandler.ClipboardHandlingIsActive)
								{
									var clipboardHandlingThread = new Thread(StartClipBoardHandling) { Name = "ClipboardHandling" };
									clipboardHandlingThread.TrySetApartmentState(ApartmentState.STA);
									clipboardHandlingThread.Start();
									StartedThreads.Add(clipboardHandlingThread);
								}
							}
							break;
						case NamedPipesSharedData.NameedPipesServerAction.RemovePID:
							if (_clipboardHandler.PIDIsCurrentlyHandled(recivedPID))
								_clipboardHandler.RemoveProcess(recivedPID);
							break;
					}
				}

				pipeServerSteam.Disconnect();
			}
		}
	}
}
