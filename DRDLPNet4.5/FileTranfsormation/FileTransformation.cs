using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using DRDLPNet4_5.WindowsAdministation;
using DRDLPRegistry;
using DRDLPNet4_5.Helpres;

namespace DRDLPNet4_5.FileTranfsormation
{
	public class FileTransformation : FileContainer
	{
		private enum FileActions
		{
			Changed,
			Created,
			Renamed,
		}

		private readonly FileSystemWatcher _fileSystemWatcher;
		private readonly List<KeyValuePair<string, FileActions>> _selectedFilesCollection;
		private readonly string _selectedFile;
		private readonly string _selectedFileDirectory;

		public bool IsFileProtected { get { return FileIsProtected(_selectedFile); } }

		public FileTransformation(string fullFilePath)
		{
			if (string.IsNullOrEmpty(fullFilePath))
				throw new ArgumentException("fullFilePath can`t be empty or null");

			if (!File.Exists(fullFilePath))
				throw new FileNotFoundException(string.Format("File not found {0}", fullFilePath));

			_selectedFile = fullFilePath;
			_selectedFileDirectory = Path.GetDirectoryName(fullFilePath);

			_selectedFilesCollection = new List<KeyValuePair<string, FileActions>>();

			_fileSystemWatcher = new FileSystemWatcher
			{
				NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
							   | NotifyFilters.FileName | NotifyFilters.DirectoryName,
				IncludeSubdirectories = false
			};

			_fileSystemWatcher.Changed += (sender, args) => HandleFileWystemWatcherEvents(args.FullPath, FileActions.Changed);
			_fileSystemWatcher.Created += (sender, args) => HandleFileWystemWatcherEvents(args.FullPath, FileActions.Created);
			_fileSystemWatcher.Renamed += (sender, args) => HandleFileWystemWatcherEvents(args.FullPath, FileActions.Renamed);
			_fileSystemWatcher.Deleted += (sender, args) =>
			{
				var selectedReccord = _selectedFilesCollection.FirstOrDefault(el => el.Key == args.FullPath);

				if (!selectedReccord.IsDefault())
					_selectedFilesCollection.Remove(selectedReccord);
			};
		}
		
		public static bool FileIsProtected(string fullFilePath)
		{
			if (string.IsNullOrEmpty(fullFilePath))
				throw new ArgumentException("fullFilePath can`t be empty or null");

			if (!File.Exists(fullFilePath))
				throw new FileNotFoundException(string.Format("File not found {0}", fullFilePath));

			using (var zipFile = ZipFile.OpenRead(fullFilePath))
			{
				return zipFile.GetEntry(FILE_TRANSFORMED_FLAG) != null;
			}
		}

		public void DecryptAndOpenFile()
		{
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

			File.WriteAllBytes(newFileName, GetSourceFileBytes(_selectedFile).ToArray());
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

			_fileSystemWatcher.Path = newTempFolder;
			_fileSystemWatcher.EnableRaisingEvents = true;
			newProcess.Start();
			newProcess.WaitForExit();

			securityRules.RemoveAccessRule(securityAccesRule);
			securityRules.AddAccessRule(new FileSystemAccessRule(UserPrincipal.Current.Name, FileSystemRights.FullControl, AccessControlType.Allow));

			createdDirectory.SetAccessControl(securityRules);
			_fileSystemWatcher.EnableRaisingEvents = false;

			ProcessFiles();

			//File.Delete(_selectedFile);
			//EncryptFile(newFileName, _selectedFile);

			File.Delete(newFileName);
			createdDirectory.Delete(true);	
		}

		public void OpenAndDecryptAfterClose()
		{
			var defaultAssociatedApplication = RegistryWork.GetDefaultAssociationProgramPath(Path.GetExtension(_selectedFile));

			if (string.IsNullOrEmpty(defaultAssociatedApplication))
				return;

			if (!File.Exists(defaultAssociatedApplication))
				defaultAssociatedApplication = Environment.ExpandEnvironmentVariables(defaultAssociatedApplication);

			if (!File.Exists(defaultAssociatedApplication))
				return;

			var newProcess = new Process
			{
				StartInfo =
				{
					FileName = defaultAssociatedApplication,
					Arguments = _selectedFile,
				}
			};
			//_fileSystemWatcher.Path = _selectedFileDirectory;
			//_fileSystemWatcher.EnableRaisingEvents = true;

			newProcess.Start();
			newProcess.WaitForExit();

			//_fileSystemWatcher.EnableRaisingEvents = false;

			//ProcessFiles();

			EncryptFile();
		}

		public void EncryptFile(string fileToEncrypt, string fullPathToNewFile, bool rewriteExistedFile = true)
		{
			if (!File.Exists(fileToEncrypt))
				throw new FileNotFoundException(string.Format("File not found {0}", fileToEncrypt));

			if (string.IsNullOrEmpty(fullPathToNewFile))
				throw new ArgumentException("expectedFullFilePath cant be empty or null");

			if (File.Exists(fullPathToNewFile) && !rewriteExistedFile)
				throw new ArgumentException(string.Format("File already exists {0}", fullPathToNewFile));

			var encryptedFile = GetSafeFile(fileToEncrypt);

			if (File.Exists(fullPathToNewFile))
				File.Delete(fullPathToNewFile);

			using (var zipFile = ZipFile.Open(fullPathToNewFile, ZipArchiveMode.Create))
			{
				foreach (var file in encryptedFile)
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

		private void HandleFileWystemWatcherEvents(string selectedFile, FileActions selectedAction)
		{
			var selectedReccord = _selectedFilesCollection.FirstOrDefault(el => el.Key == selectedFile);

			if (!selectedReccord.IsDefault())
				_selectedFilesCollection.Remove(selectedReccord);

			_selectedFilesCollection.Add(new KeyValuePair<string, FileActions>(selectedFile, selectedAction));
		}

		private void ProcessFiles()
		{
			foreach (var file in _selectedFilesCollection.Where(el => File.Exists(el.Key) && File.GetAttributes(el.Key) != FileAttributes.Temporary))
			{
				var selectedDirectory = Path.GetFileName(file.Key);
				if (string.IsNullOrEmpty(selectedDirectory))
					continue;

				EncryptFile(file.Key, _selectedFileDirectory + selectedDirectory);
				File.Delete(file.Key);
			}

			foreach (var file in _selectedFilesCollection.Where(file => File.Exists(file.Key)))
			{
				File.Delete(file.Key);
			}
		}
	}
}
