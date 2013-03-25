using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.IO.Pipes;
using DRDLPNet4_5.Cryptography;
using DRDLPNet4_5.DataSource;
using DRDLPNet4_5.FileTranfsormation;
using DRDLPNet4_5.WindowsAdministation;
using DRDLPRegistry;

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
				var random = new Random(unchecked((int) DateTime.Now.Ticks));

				switch (selectedAction)
				{
					case NamedPipesSharedData.Action.GetEncryptedFile:

						fileContainer.GetSafeFile(recivedFileName, recivedFileName, true);
						pipeWriter.WriteLine(recivedFileName);
						pipeWriter.Flush();
						pipeServerSteam.WaitForPipeDrain();

						break;
					case NamedPipesSharedData.Action.GetDecryptedFile:

						var newTempFolder = Path.GetTempPath() + random.Next();
						var newFileName = newTempFolder + Path.DirectorySeparatorChar + Path.GetFileName(recivedFileName);

						while (Directory.Exists(newTempFolder))
						{
							newTempFolder = Path.GetTempPath() + random.Next();
						}

						var internalUser = Environment.MachineName +
						                   Path.DirectorySeparatorChar +
						                   LocalUserAdministation.GetDRDLPSystemUserName;

						var securityRules = new DirectorySecurity();
						var securityAccesRule = new FileSystemAccessRule(internalUser, FileSystemRights.FullControl, AccessControlType.Allow);
						var createdDirectory = Directory.CreateDirectory(newTempFolder);

						securityRules.AddAccessRule(securityAccesRule);
						
						File.WriteAllBytes(newFileName, fileContainer.GetSourceFileBytes(recivedFileName).ToArray());

						pipeWriter.WriteLine(newFileName);
						pipeWriter.Flush();
						pipeServerSteam.WaitForPipeDrain();

						var defaultAssociatedApplication = RegistryWork.GetDefaultAssociationProgramPath(Path.GetExtension(recivedFileName));

						if (string.IsNullOrEmpty(defaultAssociatedApplication))
							return;

						createdDirectory.SetAccessControl(securityRules);

						var securityString = new SecureString();

						foreach (var userPasswordChars in LocalUserAdministation.GetDRDLPSystemUserPassword)
						{
							securityString.AppendChar(userPasswordChars);
						}

						if (!SecondLoginServiceManagement.IsSecondLoginServiceRunning)
							SecondLoginServiceManagement.StartSecondLoginService();

						if (!LocalUserAdministation.IsDRDLPSystemUserEnabled)
							LocalUserAdministation.ActivateUserAccount();

						var newProcess = new Process
							{
								StartInfo =
									{
										UserName = LocalUserAdministation.GetDRDLPSystemUserName,
										Password = securityString,
										FileName = defaultAssociatedApplication,
										Arguments = newFileName,
										Domain = Environment.MachineName,
										UseShellExecute = false,
										WorkingDirectory = createdDirectory.FullName
									}
							};

						newProcess.Start();
						newProcess.WaitForExit();

						securityRules.RemoveAccessRule(securityAccesRule);
						securityRules.AddAccessRule(new FileSystemAccessRule(UserPrincipal.Current.Name, FileSystemRights.FullControl, AccessControlType.Allow));

						createdDirectory.SetAccessControl(securityRules);

						File.Delete(recivedFileName);
						fileContainer.GetSafeFile(newFileName, recivedFileName, true);

						File.Delete(newFileName);
						createdDirectory.Delete(true);

						break;
					default:
						pipeServerSteam.Disconnect();
						break;
				}
			}
		}
	}
}
