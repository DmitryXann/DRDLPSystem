using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using DRDLPNet4_5.WindowsAdministation;
using DRDLPRegistry;

namespace DRDLPNet4_5.FileTranfsormation
{
	public class FileTransformation
	{
		private readonly string _selectedFile;
		public bool IsFileProtected { get { return FileIsProtected(_selectedFile); } }

		public FileTransformation(string fullFilePath)
		{
			if (string.IsNullOrEmpty(fullFilePath))
				throw new ArgumentException("fullFilePath can`t be empty or null");

			if (!File.Exists(fullFilePath))
				throw new FileNotFoundException(string.Format("File not found {0}", fullFilePath));

			_selectedFile = fullFilePath;
		}

		public static bool FileIsProtected(string fullFilePath)
		{
			if (string.IsNullOrEmpty(fullFilePath))
				throw new ArgumentException("fullFilePath can`t be empty or null");

			if (!File.Exists(fullFilePath))
				throw new FileNotFoundException(string.Format("File not found {0}", fullFilePath));

			using (var zipFile = ZipFile.OpenRead(fullFilePath))
			{
				return zipFile.GetEntry(FileContainer.FILE_TRANSFORMED_FLAG) != null;
			}
		}

		public void DecryptAndOpenFile()
		{
			var fileContainer = new FileContainer();
			var random = new Random(unchecked((int)DateTime.Now.Ticks));

			var newTempFolder = Path.GetTempPath() + random.Next();
			var newFileName = newTempFolder + Path.DirectorySeparatorChar + Path.GetFileName(_selectedFile);

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

			File.WriteAllBytes(newFileName, fileContainer.GetSourceFileBytes(_selectedFile).ToArray());
			File.SetAttributes(newFileName, FileAttributes.Temporary);

			var defaultAssociatedApplication = RegistryWork.GetDefaultAssociationProgramPath(Path.GetExtension(_selectedFile));

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

			File.Delete(_selectedFile);
			EncryptFile(newFileName, _selectedFile);

			File.Delete(newFileName);
			createdDirectory.Delete(true);	
		}

		public void EncryptFile(string fileToEncrypt, string fullPathToNewFile, bool rewriteExistedFile = true)
		{
			if (!File.Exists(fileToEncrypt))
				throw new FileNotFoundException(string.Format("File not found {0}", fileToEncrypt));

			if (string.IsNullOrEmpty(fullPathToNewFile))
				throw new ArgumentException("expectedFullFilePath cant be empty or null");

			if (File.Exists(fullPathToNewFile) && !rewriteExistedFile)
				throw new ArgumentException(string.Format("File already exists {0}", fullPathToNewFile));

			var fileContainer = new FileContainer();

			if (File.Exists(fullPathToNewFile))
				File.Delete(fullPathToNewFile);

			using (var zipFile = ZipFile.Open(fullPathToNewFile, ZipArchiveMode.Create))
			{
				foreach (var file in fileContainer.GetSafeFile(fileToEncrypt))
				{
					using (var zipEntrySteam = new StreamWriter(zipFile.CreateEntry(file.Key).Open()))
					{
						zipEntrySteam.Write(file.Value);
						zipEntrySteam.Close();
					}
				}
			}
		}

		public void EncryptFile()
		{
			EncryptFile(_selectedFile, _selectedFile);
		}
	}
}
