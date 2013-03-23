using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace DRDLPNet4_5.DataSource
{
	public static class SystemInformation
	{
		private static readonly Random _sysInfoRandomizer = new Random(unchecked((int)DateTime.Now.Ticks));
		private static List<string> _sysInfoCollection;
		public static string GetRandomSysInfo
		{
			get
			{
				if (_sysInfoCollection == null)
				{
					_sysInfoCollection = new List<string>
						{
							GetHDDCount.ToString(),
							GetCPUCount.ToString(),
							GetBUSNum,
							GetDMAChanelsCount.ToString(),
							GetMemoryDeviceCount.ToString(),
							GetUSBCount.ToString(),
							GetNetworkAdapterCount.ToString(),
							GetVideoControllerCount.ToString()
						};

					_sysInfoCollection.AddRange(GetHDDSerialNumber.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetHDDSignatureInfo.Where(el => !string.IsNullOrEmpty(el)));

					_sysInfoCollection.AddRange(GetCPUName.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetCPUSerialInfo.Where(el => !string.IsNullOrEmpty(el)));

					_sysInfoCollection.AddRange(GetMBManufacturer.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetMBName.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetMBSerialNumber.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetMBVersion.Where(el => !string.IsNullOrEmpty(el)));

					_sysInfoCollection.AddRange(GetBIOSVersion.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetSMBIOSVersion.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetNumericBIOSVersion.Where(el => !string.IsNullOrEmpty(el)));

					_sysInfoCollection.AddRange(GetDMAChanels.Where(el => !string.IsNullOrEmpty(el)));

					_sysInfoCollection.AddRange(GetMBDeviceInfo.Where(el => !string.IsNullOrEmpty(el)));

					_sysInfoCollection.AddRange(GetMemoryDeviceLocator.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetMemoryManufacturer.Where(el => !string.IsNullOrEmpty(el)));

					_sysInfoCollection.AddRange(GetUSBInfo.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetUSBSerialInfo.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetUSBManufacturer.Where(el => !string.IsNullOrEmpty(el)));

					_sysInfoCollection.AddRange(GetPhysicalNetworkAdapterArrayAdress.Where(el => el > 0).Select(el => el.ToString()));
					_sysInfoCollection.AddRange(GetNetworkAdapterMAC.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetNetworkAdapterManufacturer.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetNetworkAdapterName.Where(el => !string.IsNullOrEmpty(el)));

					_sysInfoCollection.AddRange(GetVideoControllerName.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetVideoControllerRAM.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetVideoControllerProcessor.Where(el => !string.IsNullOrEmpty(el)));
					_sysInfoCollection.AddRange(GetVideoControllerPNPDeviceID.Where(el => !string.IsNullOrEmpty(el)));
				}

				return _sysInfoCollection[_sysInfoRandomizer.Next(0, _sysInfoCollection.Count - 1)];
			}
		}

		#region HDDInfo
		public static IEnumerable<string> GetHDDSerialNumber { get { return GetDeviceInfo("Win32_DiskDrive", "Model").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static IEnumerable<string> GetHDDSignatureInfo { get { return GetDeviceInfo("Win32_DiskDrive", "Signature").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static uint GetHDDCount { get { return GetDeviceCount("Win32_DiskDrive"); } }
		#endregion

		#region CPUInfo
		public static IEnumerable<string> GetCPUName { get { return GetDeviceInfo("Win32_Processor", "Name").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static IEnumerable<string> GetCPUSerialInfo { get { return GetDeviceInfo("Win32_Processor", "ProcessorId").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static uint GetCPUCount { get { return GetDeviceCount("Win32_Processor"); } }
		#endregion

		#region MBInfo
		public static IEnumerable<string> GetMBManufacturer { get { return GetDeviceInfo("Win32_BaseBoard", "Manufacturer").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static IEnumerable<string> GetMBName { get { return GetDeviceInfo("Win32_BaseBoard", "Product").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static IEnumerable<string> GetMBSerialNumber { get { return GetDeviceInfo("Win32_BaseBoard", "SerialNumber").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static IEnumerable<string> GetMBVersion { get { return GetDeviceInfo("Win32_BaseBoard", "Version").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		#endregion

		#region BIOSInfo
		public static IEnumerable<string> GetBIOSVersion { get { return GetDeviceInfo("Win32_BIOS", "BIOSVersion").Where(el => el != null).Select(el => (el as string[]).Aggregate((currEl, nextEl) => currEl + " " + nextEl)); } }
		public static IEnumerable<string> GetSMBIOSVersion { get { return GetDeviceInfo("Win32_BIOS", "SMBIOSBIOSVersion").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static IEnumerable<string> GetNumericBIOSVersion { get { return GetDeviceInfo("Win32_BIOS", "Version").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		#endregion

		#region BUSInfo
		public static string GetBUSNum { get { return GetDeviceInfo("Win32_Bus", "BusNum").Where(el => el != null).Select(el => el.ToString().Trim()).Aggregate((currEl, nextEl) => currEl + nextEl); } }
		#endregion

		#region DMAInfo
		public static IEnumerable<string> GetDMAChanels { get { return GetDeviceInfo("Win32_DMAChannel", "Name").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static uint GetDMAChanelsCount { get { return GetDeviceCount("Win32_DMAChannel"); } }
		#endregion

		#region MBDeviceInfo
		public static IEnumerable<string> GetMBDeviceInfo { get { return GetDeviceInfo("Win32_OnBoardDevice", "Description").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		#endregion

		#region MemoryInfo
		public static IEnumerable<string> GetMemoryDeviceLocator { get { return GetDeviceInfo("Win32_PhysicalMemory", "DeviceLocator").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static IEnumerable<string> GetMemoryManufacturer { get { return GetDeviceInfo("Win32_PhysicalMemory", "Manufacturer").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static uint GetMemoryDeviceCount { get { return GetDeviceCount("Win32_PhysicalMemory"); } }
		#endregion

		#region USBDInfo
		public static IEnumerable<string> GetUSBInfo { get { return GetDeviceInfo("Win32_USBController", "Description").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static IEnumerable<string> GetUSBSerialInfo { get { return GetDeviceInfo("Win32_USBController", "DeviceID").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static IEnumerable<string> GetUSBManufacturer { get { return GetDeviceInfo("Win32_USBController", "Manufacturer").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static uint GetUSBCount { get { return GetDeviceCount("Win32_USBController"); } }
		#endregion

		#region NetworkAdapterInfo
		private static IEnumerable<int> GetPhysicalNetworkAdapterArrayAdress
		{
			get
			{
				var physicalAdapterArray = GetDeviceInfo("Win32_NetworkAdapter", "PhysicalAdapter").Where(el => el != null).Select(el => bool.Parse(el.ToString().Trim()));
				var physicalAdapterIndex = new List<int>();
				for (var i = 0; i < physicalAdapterArray.Count(); i++)
				{
					if (physicalAdapterArray.ElementAt(i))
					{
						physicalAdapterIndex.Add(i);
					}
				}
				return physicalAdapterIndex;
			}
		}

		public static IEnumerable<string> GetNetworkAdapterMAC 
		{ 
			get
			{
				var allMACAdress = GetDeviceInfo("Win32_NetworkAdapter", "MACAddress");
				return GetPhysicalNetworkAdapterArrayAdress.Select(allMACAdress.ElementAt).Where(el => el != null).Select(el => el.ToString().Trim());
			}
		}

		public static IEnumerable<string> GetNetworkAdapterManufacturer
		{
			get
			{
				var allNetworkAdapterManufacturer = GetDeviceInfo("Win32_NetworkAdapter", "Manufacturer");
				return GetPhysicalNetworkAdapterArrayAdress.Select(allNetworkAdapterManufacturer.ElementAt).Where(el => el != null).Select(el => el.ToString().Trim());
			}
		}

		public static IEnumerable<string> GetNetworkAdapterName
		{
			get
			{
				var allNetworkAdapterName = GetDeviceInfo("Win32_NetworkAdapter", "ProductName");
				return GetPhysicalNetworkAdapterArrayAdress.Select(allNetworkAdapterName.ElementAt).Where(el => el != null).Select(el => el.ToString().Trim());
			}
		}
		public static uint  GetNetworkAdapterCount { get { return (uint) GetPhysicalNetworkAdapterArrayAdress.Count(); } }
		#endregion

		#region VideoControllerInfo
		public static IEnumerable<string> GetVideoControllerName { get { return GetDeviceInfo("Win32_VideoController", "Name").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static IEnumerable<string> GetVideoControllerRAM { get { return GetDeviceInfo("Win32_VideoController", "AdapterRAM").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static IEnumerable<string> GetVideoControllerProcessor { get { return GetDeviceInfo("Win32_VideoController", "VideoProcessor").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static IEnumerable<string> GetVideoControllerPNPDeviceID { get { return GetDeviceInfo("Win32_VideoController", "PNPDeviceID").Where(el => el != null).Select(el => el.ToString().Trim()); } }
		public static uint GetVideoControllerCount { get { return GetDeviceCount("Win32_VideoController"); } }
		#endregion


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

		private static uint GetDeviceCount(string fromWin32Class)
		{
			try
			{
				return (uint) (new ManagementObjectSearcher("SELECT * FROM " + fromWin32Class).Get()).Cast<ManagementObject>().Count(el => el != null);
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
