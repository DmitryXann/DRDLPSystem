﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace DRDLPCore.Registry
{
	public static class RegistryWork
	{
		private const string CMD_EXE_FULL_NAME = "cmd.exe";
		private const string COMMAND_ARGUMENT_TO_HIDE_CMD_WINDOW = "/C";
		private const char COMMAND_SEPARATOR = ' ';
		private const char COMMMAND_OUTPUT_RESULT_SEPARATOR = '=';
		private const char COMMAND_OUTPUT_STRING_QUOTES_START = '"';
		private const string COMMAND_OUTPUT_STRING_END = ".exe";

		private const string REGISTRY_FILE_ASSOCIATION_KEY_PATH = @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts";
		private const string REGISTRY_BACK_UP_KEY_NAME_TO_ADD = "DRDLPBackUp";
		private const string REGISTRY_FILE_ASSOCIATION_AME_TO_ADD = "DRDLP";


		private const string REGYSTRY_CLASSES_ROOT_SUBKEYS = @"shell\open\command";
		private const string REGISTRY_CLASSES_ROOT_ASSOCIATION_ATTRIBUTE = "  %1";
		private const string REGISTRY_CLASSES_ROOT_OPEN_WITH_MENU = @"*\shellex\ContextMenuHandlers\Open With";
		private const string REGISTRY_ICON_KEY_NAME = "DefaultIcon";

		private const string REGISTRY_USER_EXTINGTION_OPEN_WITH_LIST_KEY_NAME = "OpenWithList";
		private const string REGISTRY_USER_EXTINGTION_OPEN_WITH_PROG_IDS_KEY_NAME = "OpenWithProgids";
		private const string REGISTRY_USER_EXTINGTION_USER_CHOISE_KEY_NAME = "UserChoice";
		private const string REGISTRY_USER_EXTINGTION_OPEN_WITH_PROG_IDS_VALUE_NAME = "Progid";

		private const sbyte MIN_EXTINGTION_LENGTH = 2;

		private enum CMDCommands
		{
			ASSOC,
			FTYPE
		}

		public static bool OpenWithMenuEnabled
		{
			get { return OpenWithMenuState(); }
			set { ChageStateOfOpenWithMenu(value); }
		}

		private static void ChageStateOfOpenWithMenu(bool showOpenWithMenu)
		{
			var openWithMenu = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(REGISTRY_CLASSES_ROOT_OPEN_WITH_MENU, true);

			if (openWithMenu == null)
				throw new Exception("No open with menu key found");

			if (showOpenWithMenu)
			{
				var backUpKeyValue = openWithMenu.GetValue(REGISTRY_BACK_UP_KEY_NAME_TO_ADD);

				if (backUpKeyValue != null)
				{
					openWithMenu.SetValue(string.Empty, backUpKeyValue);
					openWithMenu.DeleteValue(REGISTRY_BACK_UP_KEY_NAME_TO_ADD);
				}
			}
			else
			{
				var defaultKeyValue = openWithMenu.GetValue(string.Empty) as string;

				if (!string.IsNullOrEmpty(defaultKeyValue))
				{
					openWithMenu.SetValue(REGISTRY_BACK_UP_KEY_NAME_TO_ADD, defaultKeyValue, RegistryValueKind.String);
					openWithMenu.SetValue(string.Empty, string.Empty);
				}
			}
			Microsoft.Win32.Registry.ClassesRoot.Close();
		}

		private static bool OpenWithMenuState()
		{
			var openWithMenu = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(REGISTRY_CLASSES_ROOT_OPEN_WITH_MENU, true);
			return openWithMenu != null && string.IsNullOrEmpty(openWithMenu.GetValue(string.Empty) as string);
		}

		private static bool IsAssociationChanged(string selectedFileType)
		{
			var selectedAssociationKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REGISTRY_FILE_ASSOCIATION_KEY_PATH
				+ Path.DirectorySeparatorChar + selectedFileType + Path.DirectorySeparatorChar + REGISTRY_USER_EXTINGTION_USER_CHOISE_KEY_NAME);

			if (selectedAssociationKey == null)
				return false;

			var selectedProgIdValue = selectedAssociationKey.GetValue(REGISTRY_USER_EXTINGTION_OPEN_WITH_PROG_IDS_VALUE_NAME) as string;

			return !string.IsNullOrEmpty(selectedProgIdValue) && selectedProgIdValue.Contains(REGISTRY_FILE_ASSOCIATION_AME_TO_ADD);
		}

		private static string GetCMDCommandCutResult(CMDCommands neededCommand, string fileExtingtionOrAssociaton)
		{
			if (string.IsNullOrEmpty(fileExtingtionOrAssociaton))
				return string.Empty;

			var cmdProcess = new Process
			{
				StartInfo =
				{
					UseShellExecute = false,
					RedirectStandardOutput = true,
					FileName = Environment.SystemDirectory + Path.DirectorySeparatorChar + CMD_EXE_FULL_NAME,
					Arguments = COMMAND_ARGUMENT_TO_HIDE_CMD_WINDOW + COMMAND_SEPARATOR + neededCommand + COMMAND_SEPARATOR + fileExtingtionOrAssociaton,
					CreateNoWindow = true
				}
			};

			cmdProcess.Start();


			var outputResult = cmdProcess.StandardOutput.ReadLine();
			cmdProcess.WaitForExit();

			if (string.IsNullOrEmpty(outputResult))
				return string.Empty;

			if (neededCommand == CMDCommands.ASSOC)
				return outputResult.Substring(outputResult.IndexOf(COMMMAND_OUTPUT_RESULT_SEPARATOR) + 1);

			var startIndex = outputResult.IndexOf(COMMMAND_OUTPUT_RESULT_SEPARATOR) + 1;
			var associatedProgram = outputResult.ToLower().IndexOf(COMMAND_OUTPUT_STRING_END);

			if (outputResult[startIndex] == COMMAND_OUTPUT_STRING_QUOTES_START)
				startIndex++;

			if (associatedProgram > startIndex)
				return outputResult.Substring(startIndex, (associatedProgram + COMMAND_OUTPUT_STRING_END.Length) - startIndex);

			return string.Empty;
		}

		/// <summary>
		/// Example of formed data:
		/// (level1Key1)
		///      |
		///      +- (level1Key1 valuesCollection)
		///		 +- (level1Key1 subKey1)
		///                    |
		///                    +- (level1Key1 subKey1 subKey1)
		///		 +- (level1Key1 subKey2)
		/// (level1Key2)
		///     ...
		/// 
		/// Example of formed data in types:
		/// (KeyValuePair&lt;string == name(level1Key1), object == sub keys&gt;)
		///      |
		///      +- (IEnumerable&lt;KeyValuePair&lt;string == value name, new KeyValuePair&lt;RegistryValueKind == value kind (type), object == value data (value)&gt;&gt;&gt; - valuesCollection)
		///		 +- (KeyValuePair&lt;string == name(subKey1), object == sub keys&gt;)
		///                    |
		///                    +- (KeyValuePair&lt;string == name(subKey1), object == sub keys&gt;)
		///		 +- (KeyValuePair&lt;string == name(subKey2), object == sub keys&gt;)
		/// (KeyValuePair&lt;string == name(level1Key2), object == sub keys&gt;)
		///     ... 
		/// </summary>
		/// <param name="inputData">keys collection to build tree (can be empty)</param>
		/// <param name="keyPath">father key to build tree</param>
		/// <param name="resultAggregator">key-value tree as return  of method</param>
		/// <param name="levelNumber"></param>
		private static void GetAllSubKeysAndValuesTree(IEnumerable<string> inputData, string keyPath, ref List<object> resultAggregator, RegistryKey keyFatherKeyFullPath, int levelNumber = 0)
		{
			List<object> selectedLevel;

			if (resultAggregator.Any() && (levelNumber > 0))
			{
				selectedLevel = ((KeyValuePair<string, object>)resultAggregator.LastOrDefault()).Value as List<object>;

				for (var levelCounter = 0; levelCounter < levelNumber - 1; levelCounter++)
				{
					if (selectedLevel == null)
					{
						selectedLevel = new List<object>();
					}
					else
					{
						var neededLevel = selectedLevel.LastOrDefault();
						if (neededLevel != null)
							selectedLevel = ((KeyValuePair<string, object>)neededLevel).Value as List<object>;
					}
				}
			}
			else
			{
				var headKeys = keyFatherKeyFullPath.OpenSubKey(keyPath).GetValueNames();

				selectedLevel = resultAggregator;
				if (headKeys.Any())
				{
					selectedLevel.Insert(0, new KeyValuePair<string, object>(string.Empty, FormKeyValues(keyPath, headKeys, keyFatherKeyFullPath)));
				}
			}

			foreach (var registryKey in inputData)
			{
				var expectedKeyPath = keyPath + Path.DirectorySeparatorChar + registryKey;
				var subKey = keyFatherKeyFullPath.OpenSubKey(expectedKeyPath);

				var currentKeyValueNames = keyFatherKeyFullPath.OpenSubKey(expectedKeyPath).GetValueNames();

				selectedLevel.Add(currentKeyValueNames.Length == 0
									  ? new KeyValuePair<string, object>(registryKey, new List<object>())
									  : new KeyValuePair<string, object>(registryKey, FormKeyValues(expectedKeyPath, currentKeyValueNames, keyFatherKeyFullPath)));

				GetAllSubKeysAndValuesTree(subKey.GetSubKeyNames(), expectedKeyPath, ref resultAggregator, keyFatherKeyFullPath, levelNumber + 1);
			}
		}

		/// <summary>
		/// Used inside GetAllSubKeysAndValuesTree method
		/// </summary>
		/// <param name="expectedKeyPath"></param>
		/// <param name="currentKeyValueNames"></param>
		/// <returns></returns>
		private static IEnumerable<object> FormKeyValues(string expectedKeyPath, IEnumerable<string> currentKeyValueNames, RegistryKey keyFatherKeyFullPath)
		{
			if (string.IsNullOrEmpty(expectedKeyPath) || (currentKeyValueNames == null) || (!currentKeyValueNames.Any()))
				return new List<object>();

			return new List<object>
				{
					currentKeyValueNames.Select(el =>
							{
                                var selectedKey = keyFatherKeyFullPath.OpenSubKey(expectedKeyPath);
								return new KeyValuePair<string, object>(el,
								                                        new KeyValuePair<RegistryValueKind, object>(
									                                        selectedKey.GetValueKind(el),
									                                        selectedKey.GetValue(el)));
							}).ToList()
				};
		}

		/// <summary>
		/// Use to create reg keys and values from tree
		/// </summary>
		/// <param name="keyValueTree"></param>
		/// <param name="newHeadername"></param>
		/// <param name="keyFatherKeyFullPath"></param>
		/// <param name="nameAggregator"></param>
		private static void CreateKeyValuesTree(List<object> keyValueTree, string newHeadername, RegistryKey keyFatherKeyFullPath, string nameAggregator = null)
		{
			if (!keyValueTree.Any())
				return;

			var skipOneItemFromTree = false;

			if (string.IsNullOrEmpty(nameAggregator))
			{
				keyFatherKeyFullPath.CreateSubKey(newHeadername);
				nameAggregator = newHeadername;
			}

			var firstFatherElement = keyValueTree.FirstOrDefault() as List<object>;

			if ((firstFatherElement != null))
				skipOneItemFromTree = WriteKeyValues(firstFatherElement, keyFatherKeyFullPath, newHeadername);

			foreach (KeyValuePair<string, object> keys in skipOneItemFromTree ? keyValueTree.Skip(1) : keyValueTree)
			{
				var keysValue = (keys.Value as List<object>);

				if (keysValue == null)
					continue;

				var fisrsKey = keysValue.FirstOrDefault();
				var currentPath = nameAggregator + Path.DirectorySeparatorChar + keys.Key;

				keyFatherKeyFullPath.CreateSubKey(currentPath);

				CreateKeyValuesTree(WriteKeyValues(fisrsKey, keyFatherKeyFullPath, currentPath) ? keysValue.Skip(1).ToList() : keysValue, newHeadername, keyFatherKeyFullPath, currentPath);
			}
		}

		/// <summary>
		/// Used inside CreateKeyValuesTree method
		/// </summary>
		/// <param name="regKeyTree"></param>
		/// <param name="keyFullPath"></param>
		/// <param name="currentKeyPath"></param>
		/// <returns></returns>
		private static bool WriteKeyValues(object regKeyTree, RegistryKey keyFullPath, string currentKeyPath)
		{
			if ((regKeyTree == null) || (keyFullPath == null) || string.IsNullOrEmpty(currentKeyPath))
				return false;

			if (regKeyTree is IEnumerable<KeyValuePair<string, object>>)
			{
				foreach (var keyValue in regKeyTree as IEnumerable<KeyValuePair<string, object>>)
				{
					var keyValuesData = ((KeyValuePair<RegistryValueKind, object>)keyValue.Value);

					var expectedKey = keyFullPath.OpenSubKey(currentKeyPath, true);

					if (expectedKey != null)
						expectedKey.SetValue(keyValue.Key, keyValuesData.Value, keyValuesData.Key);
				}

				return true;
			}
			return false;
		}

		private static void CreateBackUpKeys(RegistryKey expectedFatherRootKey, RegistryKey currentAssociationKey, string expectedFileExtingtionPath, string selectedFileType)
		{
			var aggregator = new List<object>();

			GetAllSubKeysAndValuesTree(currentAssociationKey.GetSubKeyNames(), expectedFileExtingtionPath, ref aggregator, Microsoft.Win32.Registry.CurrentUser);

			expectedFatherRootKey.DeleteSubKeyTree(expectedFileExtingtionPath, false);
			expectedFatherRootKey.Flush();

			CreateKeyValuesTree(aggregator, selectedFileType + REGISTRY_BACK_UP_KEY_NAME_TO_ADD, expectedFatherRootKey.OpenSubKey(REGISTRY_FILE_ASSOCIATION_KEY_PATH, true));

		}
		public static string GetDefaultAssociationProgramPath(string selectedFileType)
		{
			if (string.IsNullOrEmpty(selectedFileType))
				throw new ArgumentException("selectedFileType can`t be empty or null");

			if (selectedFileType[0] != '.')
				throw new ArgumentException("selectedFileType need to be in full format, example: .docx");

			return GetCMDCommandCutResult(CMDCommands.FTYPE, GetCMDCommandCutResult(CMDCommands.ASSOC, selectedFileType));
		}

		public static IEnumerable<KeyValuePair<string, bool>> GetAllFileExtingtion()
		{
			var selectedKeys = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REGISTRY_FILE_ASSOCIATION_KEY_PATH);
			return selectedKeys != null
				? Microsoft.Win32.Registry.ClassesRoot.GetSubKeyNames().Except(selectedKeys.GetSubKeyNames()).Concat(selectedKeys.GetSubKeyNames())
												.AsParallel()
												.Where(el => (el.Length > MIN_EXTINGTION_LENGTH) && (el[0] == '.' && (!el.Contains(REGISTRY_BACK_UP_KEY_NAME_TO_ADD))))
												.Select(el => new KeyValuePair<string, bool>(el, IsAssociationChanged(el)))
				: null;
		}

		public static string GetDefaultAssociationImage(string selectedFileType)
		{
			if (string.IsNullOrEmpty(selectedFileType))
				throw new ArgumentException("selectedFileType can`t be empty or null");

			if (selectedFileType[0] != '.')
				throw new ArgumentException("selectedFileType need to be in full format, example: .docx");

			var extingtionNickName = GetCMDCommandCutResult(CMDCommands.ASSOC, selectedFileType);

			if (!string.IsNullOrEmpty(extingtionNickName))
			{
				var extingtionIconKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extingtionNickName + Path.DirectorySeparatorChar + REGISTRY_ICON_KEY_NAME);

				if (extingtionIconKey != null)
					return extingtionIconKey.GetValue(string.Empty).ToString();
			}
			return string.Empty;
		}

		public static void ChageFileAssociation(IEnumerable<string> selectedFileTypes, string fullPathToTheAssociatedProgram)
		{
			if (string.IsNullOrEmpty(fullPathToTheAssociatedProgram))
				throw new ArgumentException("fullPathToTheAssociatedProgram can`t be empty or null");

			if (selectedFileTypes == null)
				throw new ArgumentNullException("selectedFileTypes");

			if (!selectedFileTypes.Any())
				throw new AggregateException("selectedFileTypes cant be empty");

			foreach (var fileType in selectedFileTypes)
			{
				ChageFileAssociation(fileType, fullPathToTheAssociatedProgram);
			}
		}


		public static void ChageFileAssociation(string selectedFileType, string fullPathToTheAssociatedProgram)
		{
			if (string.IsNullOrEmpty(selectedFileType))
				throw new ArgumentException("selectedFileType can`t be empty or null");

			if (selectedFileType.ToLower().Contains(REGISTRY_BACK_UP_KEY_NAME_TO_ADD))
				throw new ArgumentException(string.Format("{0} is internal back up format", selectedFileType));

			if (selectedFileType[0] != '.')
				throw new ArgumentException("selectedFileType need to be in full format, example: .docx");

			var extingtionNickName = GetCMDCommandCutResult(CMDCommands.ASSOC, selectedFileType);
			var extingtionIconValue = GetDefaultAssociationImage(selectedFileType);
			var expectedFileExtingtionPath = REGISTRY_FILE_ASSOCIATION_KEY_PATH + Path.DirectorySeparatorChar + selectedFileType;

			var currentAssociationKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(expectedFileExtingtionPath);

			if (currentAssociationKey != null)
			{
				CreateBackUpKeys(Microsoft.Win32.Registry.CurrentUser, currentAssociationKey, expectedFileExtingtionPath, selectedFileType);
			}

			var classesRootFatherKey = (selectedFileType + REGISTRY_FILE_ASSOCIATION_AME_TO_ADD).ToUpper().Substring(1);

			var newClassesRootKey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(classesRootFatherKey + Path.DirectorySeparatorChar + REGYSTRY_CLASSES_ROOT_SUBKEYS);
			Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(classesRootFatherKey, true)
				.SetValue(string.Empty, string.IsNullOrEmpty(extingtionNickName)
													? string.Empty
													: (Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extingtionNickName).GetValue(string.Empty) ?? string.Empty));

			var newClassesRootDefaultImageKey = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(classesRootFatherKey + Path.DirectorySeparatorChar + REGISTRY_ICON_KEY_NAME);
			newClassesRootDefaultImageKey.SetValue(string.Empty, extingtionIconValue, RegistryValueKind.String);

			newClassesRootKey.SetValue(string.Empty, fullPathToTheAssociatedProgram + REGISTRY_CLASSES_ROOT_ASSOCIATION_ATTRIBUTE, RegistryValueKind.String);


			var newAssociationkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(expectedFileExtingtionPath);
			newAssociationkey.CreateSubKey(REGISTRY_USER_EXTINGTION_OPEN_WITH_LIST_KEY_NAME);

			var openWithNewKey = newAssociationkey.CreateSubKey(REGISTRY_USER_EXTINGTION_OPEN_WITH_PROG_IDS_KEY_NAME);
			openWithNewKey.SetValue(classesRootFatherKey, new byte[] { }, RegistryValueKind.None);

			var userChoiseNewKey = newAssociationkey.CreateSubKey(REGISTRY_USER_EXTINGTION_USER_CHOISE_KEY_NAME);
			userChoiseNewKey.SetValue(REGISTRY_USER_EXTINGTION_OPEN_WITH_PROG_IDS_VALUE_NAME, classesRootFatherKey, RegistryValueKind.String);

			Microsoft.Win32.Registry.CurrentUser.Close();
		}


		public static void RestoreOriginalFileAssociation(IEnumerable<string> selectedFileTypes)
		{
			if (selectedFileTypes == null)
				throw new ArgumentNullException("selectedFileTypes");

			if (!selectedFileTypes.Any())
				throw new AggregateException("selectedFileTypes cant be empty");

			foreach (var fileType in selectedFileTypes)
			{
				RestoreOriginalFileAssociation(fileType);
			}
		}

		public static void RestoreOriginalFileAssociation(string selectedFileType)
		{
			if (string.IsNullOrEmpty(selectedFileType))
				throw new ArgumentException("selectedFileType can`t be empty or null");

			if (selectedFileType.ToLower().Contains(REGISTRY_BACK_UP_KEY_NAME_TO_ADD))
				throw new ArgumentException(string.Format("{0} is internal back up format", selectedFileType));

			if (selectedFileType[0] != '.')
				throw new ArgumentException("selectedFileType need to be in full format, example: .docx");

			if (!IsAssociationChanged(selectedFileType))
				throw new ArgumentException(string.Format("{0} association isn`t changed by this software", selectedFileType));

			var fullPathToCurrentAssociationKey = REGISTRY_FILE_ASSOCIATION_KEY_PATH + Path.DirectorySeparatorChar + selectedFileType;
			var expectedFileExtingtionPath = fullPathToCurrentAssociationKey + REGISTRY_BACK_UP_KEY_NAME_TO_ADD;

			var currentAssociationKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(expectedFileExtingtionPath);

			if (currentAssociationKey == null)
			{
				Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(fullPathToCurrentAssociationKey, false);
				Microsoft.Win32.Registry.CurrentUser.Close();
			}

			var associationPseudonim = (selectedFileType + REGISTRY_FILE_ASSOCIATION_AME_TO_ADD).ToUpper().Substring(1);

			Microsoft.Win32.Registry.ClassesRoot.DeleteSubKeyTree(associationPseudonim, false);

			var aggregator = new List<object>();

			GetAllSubKeysAndValuesTree(currentAssociationKey.GetSubKeyNames(), expectedFileExtingtionPath, ref aggregator, Microsoft.Win32.Registry.CurrentUser);

			var openWithList = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(fullPathToCurrentAssociationKey + Path.DirectorySeparatorChar + REGISTRY_USER_EXTINGTION_OPEN_WITH_PROG_IDS_KEY_NAME, true);

			if (openWithList != null)
				openWithList.DeleteValue(associationPseudonim);

			Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(expectedFileExtingtionPath, false);
			Microsoft.Win32.Registry.CurrentUser.Flush();

			CreateKeyValuesTree(aggregator, selectedFileType, Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REGISTRY_FILE_ASSOCIATION_KEY_PATH, true));
			Microsoft.Win32.Registry.CurrentUser.Close();
		}
	}
}
