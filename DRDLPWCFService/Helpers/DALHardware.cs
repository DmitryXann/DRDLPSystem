using System;
using DRDLPSystemDAL;
using DRDLPSystemDAL.Interfaces;

namespace DRDLPWCFService.Helpers
{
	public class DALHardware : IHardware
	{
		public string HardwareID { get; set; }
		public string Name { get; set; }
		public string OtherInfo { get; set; }
		public HardwareTypeEnum Type { get; set; }

		public DALHardware(string hardwareID, string name, string otherInfo, HardwareTypeEnum hardwareType)
		{
			if (string.IsNullOrEmpty(hardwareID))
				throw new ArgumentException("hardwareID can`t be empty or null");

			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("name can`t be empty or null");

			HardwareID = hardwareID;
			Name = name;
			OtherInfo = string.IsNullOrEmpty(otherInfo) ? string.Empty : otherInfo;
			Type = hardwareType;
		}
	}
}
