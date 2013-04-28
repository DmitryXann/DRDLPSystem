using System;
using System.Collections.Generic;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddHardware(Hardware hardware)
		{
			if (hardware == null)
				throw new ArgumentNullException("hardware");

			if (hardware.PC == null)
				throw new ArgumentNullException("hardware.PC");

			if (!_container.PCSet.Contains(hardware.PC))
				throw new ArgumentException("hardware.PC do not contains in DB");

			hardware.HardwareID = hardware.HardwareID.ToLower().Trim();
			hardware.Name = hardware.Name.ToLower().Trim();
			hardware.OtherInfo = hardware.OtherInfo.ToLower().Trim();
			hardware.PC = _container.PCSet.Attach(hardware.PC);

			_container.HardwareSet.Add(hardware);
		}

		public void AddHardware(string hardwareID, string name, string otherInfo, HardwareTypeEnum type, PC pc)
		{
			if (string.IsNullOrEmpty(hardwareID) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(otherInfo))
				throw new ArgumentException("hardwareID or name or otherInfo can`t be empty or null");

			if (pc == null)
				throw new ArgumentNullException("hardware.PC");

			if (!_container.PCSet.Contains(pc))
				throw new ArgumentException("hardware.PC do not contains in DB");

			_container.HardwareSet.Add(new Hardware { Name = name.ToLower().Trim(), 
													  HardwareID = hardwareID.ToLower().Trim(), 
													  OtherInfo = otherInfo.ToLower().Trim(), 
													  Type = type ,
													  PC = _container.PCSet.Attach(pc)});
		}
		
		public void RemoveHardware(Hardware hardware)
		{
			if (hardware == null)
				throw new ArgumentNullException("hardware");

			_container.HardwareSet.Remove(_container.HardwareSet.Attach(hardware));
		}

		public Hardware AttachHardware(Hardware hardware)
		{
			if (hardware == null)
				throw new ArgumentNullException("hardware");

			return _container.HardwareSet.Attach(hardware);
		}

		public Hardware GetHardwareByID(int hardwareId)
		{
			return _container.HardwareSet.FirstOrDefault(el => el.Id == hardwareId);
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

		public IEnumerable<Hardware> GetAllHardware()
		{
			return _container.HardwareSet.ToArray();
		}

		public long GetHardwareCount()
		{
			return _container.HardwareSet.LongCount();
		}
	}
}
