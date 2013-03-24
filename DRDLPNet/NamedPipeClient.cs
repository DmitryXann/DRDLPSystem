using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace DRDLPClientOnlyNet4
{
	public class NamedPipeClientNet4
	{
		private const string SERVER_NAME = ".";

		public enum Action
		{
			GetEncryptedFile,
			GetDecryptedFile,
		}

		private static string _lisenerServerPipeName;
		private static string LisenerServerPipeName
		{
			get
			{
				try
				{
					if (string.IsNullOrEmpty(_lisenerServerPipeName))
					{
						_lisenerServerPipeName = GetMD5HashSum(GetCPUSerialInfo.First(el => !string.IsNullOrEmpty(el)));
					}
					return _lisenerServerPipeName;
				}
				catch (Exception)
				{
					if (string.IsNullOrEmpty(_lisenerServerPipeName))
					{
						_lisenerServerPipeName = GetMD5HashSum(DateTime.Now.Year.ToString() + DateTime.Now.Month);
					}
					return _lisenerServerPipeName;
				}
			}
		}

		private static IEnumerable<string> GetCPUSerialInfo { get { return GetDeviceInfo("Win32_Processor", "ProcessorId").Where(el => el != null).Select(el => el.ToString().Trim()); } }

		public delegate void FileReadySignature(string fileName, Action selectedAction);
		public event FileReadySignature FileReady;

		private static IEnumerable<object> GetDeviceInfo(string fromWin32Class, string classItemAdd)
		{
			try
			{
				return from ManagementObject obj in new ManagementObjectSearcher("SELECT * FROM " + fromWin32Class).Get() select obj[classItemAdd];
			}
			catch (Exception)
			{
				throw;
			}
		}

		private static string GetMD5HashSum(string inputData)
		{
			if (string.IsNullOrEmpty(inputData))
				throw new ArgumentException("inputData mast have value");

			using (var md5 = MD5.Create())
			{
				return md5.ComputeHash(Encoding.ASCII.GetBytes(inputData))
						  .Select(el => el.ToString("x2"))
						  .Aggregate((curEl, nextEl) => curEl + nextEl);
			}
		}
		
		public static string GetPerconalConversationPipeName()
		{
			using (var namingPipeClient = new NamedPipeClientStream(SERVER_NAME, LisenerServerPipeName))
			{
				namingPipeClient.Connect();
				var pipeWriter = new StreamWriter(namingPipeClient);
				var pipeReader = new StreamReader(namingPipeClient);

				pipeWriter.WriteLine(LisenerServerPipeName);
				pipeWriter.Flush();
				namingPipeClient.WaitForPipeDrain();
				
				var readedName = pipeReader.ReadLine();
				
				return readedName;
			}
		}

		public void StartConversation(string personalPipeName, string fileName,  Action selectedAction)
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
