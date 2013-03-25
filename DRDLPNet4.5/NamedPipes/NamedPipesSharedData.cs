using System;
using System.Linq;
using DRDLPNet4_5.Cryptography;
using DRDLPNet4_5.DataSource;

namespace DRDLPNet4_5.NamedPipes
{
	public static class NamedPipesSharedData
	{
		/*private const byte AES_BLOCK_SIZE = 128;
		private const byte AES_TAKE_KEY_ELEMENT_NUMBER = 32;
		private const byte AES_TAKE_IV_ELEMENT_NUMBER = 16;

		private static readonly byte[] AES_KEY_VALUE = Encoding.ASCII.GetBytes(DataCryptography.GetHashSum(LisenerServerPipeName, DataCryptography.HashSum.Sha512)).Take(AES_TAKE_KEY_ELEMENT_NUMBER).ToArray();
		private static readonly byte[] AES_IV_VALUE = Encoding.ASCII.GetBytes(DataCryptography.GetHashSum(LisenerServerPipeName + SystemInformation.GetRandomSysInfo, DataCryptography.HashSum.Sha512)).Take(AES_TAKE_KEY_ELEMENT_NUMBER).ToArray();
		
		internal static string GetEncryptedMessage(string messageToEncrypt)
		{
			return string.Empty;
		}

		internal static string GetDecryptedMEssage(string encryptedMessage)
		{
			return string.Empty;
		} 
		 */

		public enum Action
		{
			GetEncryptedFile,
			GetDecryptedFile,
			//FileReady
		}

		private static string _lisenerServerPipeName;
		internal static string LisenerServerPipeName
		{
			get
			{
				try
				{
					if (string.IsNullOrEmpty(_lisenerServerPipeName))
					{
						_lisenerServerPipeName = DataCryptography.GetHashSum(SystemInformation.GetCPUSerialInfo.First(el => !string.IsNullOrEmpty(el)), DataCryptography.HashSum.Md5);
					}
					return _lisenerServerPipeName;
				}
				catch (Exception)
				{
					if (string.IsNullOrEmpty(_lisenerServerPipeName))
					{
						_lisenerServerPipeName = DataCryptography.GetHashSum(DateTime.Now.Year.ToString() + DateTime.Now.Month, DataCryptography.HashSum.Md5);
					}
					return _lisenerServerPipeName;
				}
			}
		}

	}
}
