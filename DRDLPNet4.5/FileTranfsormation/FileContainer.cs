using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using DRDLPNet4_5.Cryptography;
using DRDLPNet4_5.DataSource;

namespace DRDLPNet4_5.FileTranfsormation
{
	/// <summary>
	/// TODO: no exception handling
	/// TODO: no custom constructor for custom hardware
	/// TODO: no file naming randomization, consider implementation???? 
	/// </summary>
	public class FileContainer
	{
		private const sbyte ONE_HEX_MAX_ELEMENT_COUNT = 4;
		private const sbyte NUMERIC_BASE = 16;

		private const byte AES_BLOCK_SIZE = 128;
		private const byte AES_TAKE_KEY_ELEMENT_NUMBER = 32;
		private const byte AES_TAKE_IV_ELEMENT_NUMBER = 16;

		private const sbyte FILE_SIZE_TAKE_ELEMENTS_FROM_HASH = 3;

		#region HARDWARE info
		private static string CpuSerial
		{
			get
			{
				try
				{
					var cpuSerial = SystemInformation.GetCPUSerialInfo;
					if (cpuSerial.Any())
						return string.IsNullOrEmpty(cpuSerial.First())
							       ? string.Empty
							       : cpuSerial.First();
					return string.Empty;

				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}
		private static string CPUName
		{
			get
			{
				try
				{
					var cpuName = SystemInformation.GetCPUName;
					if (cpuName.Any())
						return string.IsNullOrEmpty(cpuName.First())
							       ? string.Empty
							       : cpuName.First();
					return string.Empty;

				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}

		private static string MBSerial
		{
			get
			{
				try
				{
					var mbSerial = SystemInformation.GetMBSerialNumber;
					if (mbSerial.Any())
						return string.IsNullOrEmpty(mbSerial.First())
							       ? string.Empty
							       : mbSerial.First();
					return string.Empty;
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}
		private static string MBName
		{
			get
			{
				try
				{
					var mbName = SystemInformation.GetMBName;
					if (mbName.Any())
						return string.IsNullOrEmpty(mbName.First())
							       ? string.Empty
							       : mbName.First();
					return string.Empty;

				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}

		private static string VGAID
		{
			get
			{
				try
				{
					var vgaID = SystemInformation.GetVideoControllerPNPDeviceID;
					if (vgaID.Any())
						return string.IsNullOrEmpty(vgaID.First())
							       ? string.Empty
							       : vgaID.First();
					return string.Empty;

				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}

		private static string USBID
		{
			get
			{
				try
				{
					var usbSerial = SystemInformation.GetCPUSerialInfo;
					if (usbSerial.Any())
						return string.IsNullOrEmpty(usbSerial.First())
							       ? string.Empty
							       : usbSerial.First();
					return string.Empty;

				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}
		#endregion

		private readonly int _filesSizeInfoes;
		private readonly byte[] _aesKeyValue;
		private readonly byte[] _aesIvValue;
		
		private uint _startedCretionThreads;
		private uint _startedReCretionThreads;
		private ConcurrentBag<KeyValuePair<int, string>> _preperadFiles;
		private ConcurrentBag<KeyValuePair<int, List<byte>>> _sourceDecryptedFileData;
 
		public FileContainer()
		{
			_filesSizeInfoes = int.Parse(DataCryptography.GetHashSum(CpuSerial + MBName, DataCryptography.HashSum.Md5).Substring(0, FILE_SIZE_TAKE_ELEMENTS_FROM_HASH), NumberStyles.HexNumber);
			_aesKeyValue = Encoding.ASCII.GetBytes(DataCryptography.GetHashSum(CpuSerial + MBSerial + USBID, DataCryptography.HashSum.Sha512)).Take(AES_TAKE_KEY_ELEMENT_NUMBER).ToArray();
			_aesIvValue = Encoding.ASCII.GetBytes(DataCryptography.GetHashSum(CPUName + VGAID + CpuSerial, DataCryptography.HashSum.Sha512)).Take(AES_TAKE_IV_ELEMENT_NUMBER).ToArray();
		}

		#region Threads
		private void StartFileCreation(object inputData)
		{
			var data = (KeyValuePair<int, string>)inputData;
			_preperadFiles.Add(new KeyValuePair<int, string>(data.Key, ByteArrayToHexString(DataCryptography.GetAESEncryptedMessage(data.Value, _aesKeyValue, _aesIvValue, AES_BLOCK_SIZE))));
			_startedCretionThreads--;
		}

		private void StartFileReCreation(object inputData)
		{
			var data = (KeyValuePair<int, string>) inputData;
			_sourceDecryptedFileData.Add(new KeyValuePair<int, List<byte>>(data.Key,
				HexStringToByteList(DataCryptography.GetAESDecryptedMessage(HexStringToByteList(data.Value).ToArray(), _aesKeyValue, _aesIvValue, AES_BLOCK_SIZE).Trim(DataCryptography.CHARS_TO_TRIM))));
			_startedReCretionThreads--;
		}
		#endregion

		//TODO: test for string vs StrungBuilder
		private static List<byte> HexStringToByteList(string selectedData)
		{
			if (string.IsNullOrEmpty(selectedData))
				return null;

			var sourceDecryptedFileData = new List<byte>();
			for (var counter = 0; counter < selectedData.Length; counter += ONE_HEX_MAX_ELEMENT_COUNT)
			{
				var accumulator = string.Empty;
				//var accumulator = new StringBuilder();
					
				for (var subCounter = counter; subCounter < counter + ONE_HEX_MAX_ELEMENT_COUNT; subCounter++)
				{
					accumulator += selectedData[subCounter];
					//accumulator.Append(selectedData[subCounter]);
				}
				sourceDecryptedFileData.Add(Convert.ToByte(accumulator/*.ToString()*/, NUMERIC_BASE));
			}

			return sourceDecryptedFileData;
		}
		//TODO: test for string vs StrungBuilder
		private static string ByteArrayToHexString(IEnumerable<byte> sourceData)
		{
			//var outputData = new StringBuilder();
			var outputData = string.Empty;
			//TODO: test for linq time
			foreach (var hexData in sourceData.Select(el => Convert.ToString(el, NUMERIC_BASE).PadLeft(ONE_HEX_MAX_ELEMENT_COUNT, '0')))
			{
				outputData += hexData;
				//outputData.Append(hexData);
			}

			return outputData/*.ToString()*/;
		}

		/// <summary>
		/// TODO: method return?????
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="expectedFullFilePath"></param>
		/// <param name="rewriteExistedFile"></param>
		/// <returns></returns>
		public string GetSafeFile(string fileName, string expectedFullFilePath, bool rewriteExistedFile)
		{
			if (!File.Exists(fileName))
				throw new FileNotFoundException(string.Format("File not found {0}", fileName));

			if (string.IsNullOrEmpty(expectedFullFilePath))
				throw new ArgumentException("expectedFullFilePath cant be empty or null");

			if (File.Exists(expectedFullFilePath) && !rewriteExistedFile)
				throw new ArgumentException(string.Format("File already exists {0}", expectedFullFilePath));

			_preperadFiles = new ConcurrentBag<KeyValuePair<int, string>>();
			var inputFileBits = File.ReadAllBytes(fileName).Select(el => Convert.ToString(el, NUMERIC_BASE).PadLeft(ONE_HEX_MAX_ELEMENT_COUNT, '0')).ToList();
			var inputFileBitsCount = inputFileBits.Count();

			var totalCounter = 0;
			int counter;

			var accumulator = new StringBuilder();

			for (counter = 0; (counter + _filesSizeInfoes) <= inputFileBitsCount; counter += _filesSizeInfoes)
			{
				for (var subCount = counter; subCount < counter + _filesSizeInfoes; subCount++)
					accumulator.Append(inputFileBits[subCount]);

				var newThread = new Thread(StartFileCreation) {Priority = ThreadPriority.Normal};
				newThread.Start(new KeyValuePair<int, string>(totalCounter++, accumulator.ToString()));

				_startedCretionThreads++;
				accumulator.Clear();
				
				if ((counter + _filesSizeInfoes * 2) < inputFileBitsCount)
				{
					counter += _filesSizeInfoes;
					for (var subCount = counter; subCount < counter + _filesSizeInfoes; subCount++)
						accumulator.Append(inputFileBits[subCount]);

					_preperadFiles.Add(new KeyValuePair<int, string>(totalCounter++, ByteArrayToHexString(DataCryptography.GetAESEncryptedMessage(accumulator.ToString(), _aesKeyValue, _aesIvValue, AES_BLOCK_SIZE))));
					accumulator.Clear();
				}
				 
			}
			
			if (counter < inputFileBitsCount)
			{
				for (var subCount = counter; subCount < inputFileBitsCount; subCount++)
					accumulator.Append(inputFileBits[subCount]);
				_preperadFiles.Add(new KeyValuePair<int, string>(totalCounter, ByteArrayToHexString(DataCryptography.GetAESEncryptedMessage(accumulator.ToString(), _aesKeyValue, _aesIvValue, AES_BLOCK_SIZE))));
			}


			if (File.Exists(expectedFullFilePath))
				File.Delete(expectedFullFilePath);

			using (var zipFile = ZipFile.Open(expectedFullFilePath, ZipArchiveMode.Create))
			{
				while (_startedCretionThreads > 0)
					Thread.Sleep(1);

				foreach (var file in _preperadFiles)
				{
					using (var zipEntrySteam = new StreamWriter(zipFile.CreateEntry(file.Key.ToString()).Open()))
					{
						zipEntrySteam.Write(file.Value);
						zipEntrySteam.Close();
					}
				}
			}
			return expectedFullFilePath;
		}
		
		public IEnumerable<byte> GetSourceFileBytes(string fileName)
		{
			if (!File.Exists(fileName))
				throw new FileNotFoundException(string.Format("File not found {0}", fileName));

			_sourceDecryptedFileData = new ConcurrentBag<KeyValuePair<int, List<byte>>>();

			var accumulator = new List<string>();
			using (var zipFile = ZipFile.OpenRead(fileName))
			{
				foreach (var zipEntry in zipFile.Entries.OrderBy(el => uint.Parse(el.Name)))
				{
					using (var zipEntryDataSteam = new StreamReader(zipEntry.Open()))
					{
						accumulator.Add(zipEntryDataSteam.ReadToEnd());
					}
				}
			}
			
			var accumulatorCount = accumulator.Count;
			int index;
			for (index = 0; index < accumulatorCount - 1; index++)
			{
				var newThread = new Thread(StartFileReCreation);
				newThread.Start(new KeyValuePair<int, string>(index, accumulator[index]));
				_startedReCretionThreads++;
			}

			_sourceDecryptedFileData.Add(new KeyValuePair<int, List<byte>>(index,
				HexStringToByteList(DataCryptography.GetAESDecryptedMessage(HexStringToByteList(accumulator[index]).ToArray(), _aesKeyValue, _aesIvValue, AES_BLOCK_SIZE).Trim(DataCryptography.CHARS_TO_TRIM))));
			
			while (_startedReCretionThreads > 0)
				Thread.Sleep(1);

			var fileBytes = new List<byte>();

			foreach (var fileBytesPart in _sourceDecryptedFileData.OrderBy(el => el.Key).Select(el => el.Value))
				fileBytes.AddRange(fileBytesPart);

			return fileBytes;
		}
	}
}
