using System;
using System.IO;
using System.IO.Pipes;

namespace DRDLPNet4_5.NamedPipes
{
	public static class NamedPipeClient
	{
		private const string SERVER_NAME = ".";
		
		public delegate void FileReadySignature(string fileName, NamedPipesSharedData.Action selectedAction);
		public static event FileReadySignature FileReady;

		public static string GetPerconalConversationPipeName()
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
				
				return readedName;
			}
		}

		public static void StartConversation(string personalPipeName, string fileName,  NamedPipesSharedData.Action selectedAction)
		{
			if (string.IsNullOrEmpty(personalPipeName))
				throw new ArgumentException("personalPipeName can`t be empty or null");

			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentException("fileName can`t be empty or null");

			using (var namingPipeClient = new NamedPipeClientStream(SERVER_NAME, personalPipeName, PipeDirection.InOut))
			{
				namingPipeClient.Connect();
				var pipeWriter = new StreamWriter(namingPipeClient);
				var pipeReader = new StreamReader(namingPipeClient);

				var readedData = pipeReader.ReadLine();

				if (string.IsNullOrEmpty(readedData) || !readedData.Equals(personalPipeName))
					return;

				pipeWriter.WriteLine(fileName);
				pipeWriter.Flush();
				namingPipeClient.WaitForPipeDrain();

				pipeWriter.WriteLine(selectedAction.ToString());
				pipeWriter.Flush();
				namingPipeClient.WaitForPipeDrain();

				var fileIsReady = pipeReader.ReadLine();
				if ((FileReady != null) && !string.IsNullOrEmpty(fileIsReady))
					FileReady(fileIsReady, selectedAction);
			}
		}
	}
}
