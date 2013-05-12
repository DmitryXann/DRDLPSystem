using System;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddHardware(string hardwareID, string name, string otherInfo, HardwareTypeEnum type, PC pc)
		{
			if (string.IsNullOrEmpty(hardwareID) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(otherInfo))
				throw new ArgumentException("hardwareID or name or otherInfo can`t be empty or null");

			if (pc == null)
				throw new ArgumentNullException("hardware.PC");

			if (!_container.PCSet.Any(el => el.Id == pc.Id))
				throw new ArgumentException("hardware.PC do not contains in DB");

			_container.HardwareSet.Add(new Hardware { Name = name.ToLower().Trim(), 
													  HardwareID = hardwareID.ToLower().Trim(), 
													  OtherInfo = otherInfo.ToLower().Trim(), 
													  Type = type ,
													  PC = _container.PCSet.Attach(pc)});
		}
		
		public Hardware GetHardwareByHardwareID(string hardwareID)
		{
			if (string.IsNullOrEmpty(hardwareID))
				throw new ArgumentException("hardwareID can`t be empty or null");

			return _container.HardwareSet.FirstOrDefault(el => el.HardwareID.ToLower().Trim() == hardwareID.ToLower().Trim());
		}

		public Hardware GetHardwareByHardwareInfo(string hardwareID, string name, string otherInfo, HardwareTypeEnum type)
		{
			if (string.IsNullOrEmpty(hardwareID) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(otherInfo))
				throw new ArgumentException("hardwareID or name or otherInfo can`t be empty or null");

			return _container.HardwareSet.FirstOrDefault(el => (el.HardwareID.ToLower().Trim() == hardwareID.ToLower().Trim()) &&
			                                                   (el.Name.ToLower().Trim() == name.ToLower().Trim()) &&
			                                                   (el.OtherInfo.ToLower().Trim() == otherInfo.ToLower().Trim()) &&
			                                                   (el.Type == type));
		}
	}
}
