﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.IO.Pipes;
using DRDLPNet4_5.Cryptography;
using DRDLPNet4_5.DataSource;
using DRDLPNet4_5.FileTranfsormation;

namespace DRDLPNet4_5.NamedPipes
{
	public static class NamedPipeServer
	{
		private const int LISENER_THREAD_SLEEP_MILI_SECONDS = 300;
		public static bool ServerStatus { get; set; }

		private static int? _lisenerThreadSleepMSeconds;
		public static int LisenerThreadSleepMSeconds
		{
			get { return _lisenerThreadSleepMSeconds.HasValue ? _lisenerThreadSleepMSeconds.Value : LISENER_THREAD_SLEEP_MILI_SECONDS; } 
			set { _lisenerThreadSleepMSeconds = value; }
		}
   
		public static void StartPipeServer()
		{
			ServerStatus = true;
			var lisenerThread = new Thread(ConnectionLisener);
			lisenerThread.Start();
		}

		public static void StopPipeServer()
		{
			ServerStatus = false;
		}

		private static void ConnectionLisener()
		{
			using (var pipeServerSteam = new NamedPipeServerStream(NamedPipesSharedData.LisenerServerPipeName, PipeDirection.InOut))
			{
				while (ServerStatus)
				{
					Thread.Sleep(LisenerThreadSleepMSeconds);
					pipeServerSteam.WaitForConnection();

					var pipeReader = new StreamReader(pipeServerSteam);
					var pipeClientRequest = pipeReader.ReadLine();

					if (string.IsNullOrEmpty(pipeClientRequest) || !pipeClientRequest.Equals(NamedPipesSharedData.LisenerServerPipeName))
					{
						pipeServerSteam.Disconnect();
						continue;
					}

					var pipeWriter = new StreamWriter(pipeServerSteam);
					var selectedPipeName = DataCryptography.GetHashSum(SystemInformation.GetRandomSysInfo + DateTime.Now.Ticks, DataCryptography.HashSum.Md5);

					pipeWriter.WriteLine(selectedPipeName);
					pipeWriter.Flush();
					pipeServerSteam.WaitForPipeDrain();

					var newConversationThread = new Thread(ConversationThread);
					newConversationThread.Start(selectedPipeName);

					pipeServerSteam.Disconnect();
				}
			}
		}


		private static void ConversationThread(object conversationThreadPipeName)
		{
			var selectedPipeName = conversationThreadPipeName as string;

			if (string.IsNullOrEmpty(selectedPipeName))
				return;

			using (var pipeServerSteam = new NamedPipeServerStream(selectedPipeName, PipeDirection.InOut, 1))
			{
				pipeServerSteam.WaitForConnection();
				
				var pipeReader = new StreamReader(pipeServerSteam);
				var pipeWriter = new StreamWriter(pipeServerSteam);

				pipeWriter.WriteLine(selectedPipeName);
				pipeWriter.Flush();
				pipeServerSteam.WaitForPipeDrain();


				var recivedFileName = pipeReader.ReadLine();

				if (!File.Exists(recivedFileName))
				{
					pipeServerSteam.Disconnect();
					return;
				}

				NamedPipesSharedData.Action selectedAction;

				if (!NamedPipesSharedData.Action.TryParse(pipeReader.ReadLine(), out selectedAction))
				{
					pipeServerSteam.Disconnect();
					return;
				}
				var fileContainer = new FileContainer();
				
				switch (selectedAction)
				{
					case NamedPipesSharedData.Action.GetEncryptedFile:
						var newFileName = recivedFileName + new Random(unchecked((int) DateTime.Now.Ticks)).Next() + "." + "doc";
						fileContainer.GetSafeFile(recivedFileName, newFileName, true);
						pipeWriter.WriteLine(newFileName);
						pipeWriter.Flush();
						pipeServerSteam.WaitForPipeDrain();
						break;
					case NamedPipesSharedData.Action.GetDecryptedFile:
						var newFile = recivedFileName +  new Random(unchecked((int) DateTime.Now.Ticks)).Next() + "." + "docx";
						File.WriteAllBytes(newFile, fileContainer.GetSourceFileBytes(recivedFileName).ToArray());
						pipeWriter.WriteLine(newFile);
						pipeWriter.Flush();
						pipeServerSteam.WaitForPipeDrain();
						break;
					default:
						pipeServerSteam.Disconnect();
						break;
				}
			}
		}
	}
}