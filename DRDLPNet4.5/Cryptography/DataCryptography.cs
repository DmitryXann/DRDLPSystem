using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DRDLPNet4_5.Cryptography
{
	public static class DataCryptography
	{
		public enum HashSum
		{
			Md5,
			Sha1,
			Sha256,
			Sha512
		}

		public static readonly char[] CHARS_TO_TRIM = new[] { '\0' };

		public static string GetHashSum(string inputData, HashSum selectedHashSum)
		{
			if (string.IsNullOrEmpty(inputData))
				throw new ArgumentException("inputData mast have value");

			switch (selectedHashSum)
			{
				case HashSum.Md5:
					{
						using (var md5 = MD5.Create())
						{
							return md5.ComputeHash(Encoding.ASCII.GetBytes(inputData))
							          .Select(el => el.ToString("x2"))
							          .Aggregate((curEl, nextEl) => curEl + nextEl);}
					}
				case HashSum.Sha1:
					{
						using (var sha1 = SHA1.Create())
						{
							return sha1.ComputeHash(Encoding.ASCII.GetBytes(inputData))
							           .Select(el => el.ToString("x2"))
							           .Aggregate((curEl, nextEl) => curEl + nextEl);}
					}
				case HashSum.Sha256:
					{
						using (var sha256 = SHA256.Create())
						{
							return sha256.ComputeHash(Encoding.ASCII.GetBytes(inputData))
							             .Select(el => el.ToString("x2"))
							             .Aggregate((curEl, nextEl) => curEl + nextEl);}
					}
				case HashSum.Sha512:
					{
						using (var sha256 = SHA512.Create())
						{
							return sha256.ComputeHash(Encoding.ASCII.GetBytes(inputData))
							             .Select(el => el.ToString("x2"))
							             .Aggregate((curEl, nextEl) => curEl + nextEl);}
					}
				default:
					return string.Empty;
			}

		}

		public static byte[] GetAESEncryptedMessage(string message, byte[] key, byte[] iv, int blockSize)
		{
			if (string.IsNullOrEmpty(message))
				return null;

			if ((key == null) || (key.Length <= 0))
				throw new ArgumentException("key cant be null or empty");

			if ((iv == null) || (iv.Length <= 0))
				throw new ArgumentException("iv cant be null or empty");

			using (var aes = new AesManaged())
			{
				aes.BlockSize = blockSize;
				aes.Key = key;
				aes.IV = iv;
				aes.Padding = PaddingMode.Zeros;

				using (var encryptMemorySteam = new MemoryStream())
				{
					using (var encryptCrypoSteam = new CryptoStream(encryptMemorySteam, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
					{
						using (var encryptStreamWriter = new StreamWriter(encryptCrypoSteam))
						{
							encryptStreamWriter.Write(message);
						}
					}

					return encryptMemorySteam.ToArray();
				}
			}
		}

		public static string GetAESDecryptedMessage(byte[] message, byte[] key, byte[] iv, int blockSize)
		{
			if ((message == null) || (message.Length <= 0))
				return string.Empty;

			if ((key == null) || (key.Length <= 0))
				throw new ArgumentException("key cant be null or empty");

			if ((iv == null) || (iv.Length <= 0))
				throw new ArgumentException("iv cant be null or empty");

			using (var aes = new AesManaged())
			{
				aes.BlockSize = blockSize;
				aes.Key = key;
				aes.IV = iv;
				aes.Padding = PaddingMode.Zeros;

				using (var decryptMemorySteam = new MemoryStream(message))
				{
					using (var decryptCryptoSteam = new CryptoStream(decryptMemorySteam, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read))
					{
						using (var decryptSteam = new StreamReader(decryptCryptoSteam))
						{
							return decryptSteam.ReadToEnd();
						}
					}
				}
			}
		}
	}
}
