using System;
using System.Linq;

namespace DRDLPNet4_5.NamedPipes
{
	public static class NamedPipesSharedData
	{
		public enum NameedPipesServerAction
		{
			AddPID,
			RemovePID,
			StopClipboardHandling,
			ReceiveError
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
