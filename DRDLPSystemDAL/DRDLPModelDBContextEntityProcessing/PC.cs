using System;
using System.Collections.Generic;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddPC(IEnumerable<Hardware> hardwares, string PCHardwareBasedID, string name, bool valid = true)
		{
			if (hardwares == null)
				throw new ArgumentNullException("hardwares");

			if (!hardwares.Any())
				throw new ArgumentException("pc.Hardware cant be empty");

			if (!hardwares.All(el => _container.HardwareSet.Any(elem => elem.Id == el.Id)))
				throw new ArgumentException("all pc.Hardware entities must be in DB");

			if (string.IsNullOrEmpty(PCHardwareBasedID))
				throw new ArgumentException("PCHardwareBasedID can`t be empty or null");

			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("name can`t be empty or null");

			var newPC = new PC { PCHardwareBasedID = PCHardwareBasedID.ToLower().Trim(), Valid = valid, Name = name };

			foreach (var hardware in hardwares)
			{
				newPC.Hardware.Add(_container.HardwareSet.Attach(hardware));
			}

			_container.PCSet.Add(newPC);
		}

		public void ChangePCValidation(PC pc,bool isPCValid)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");

			if (!_container.PCSet.Any(el => el.Id == pc.Id))
				throw new ArgumentException("No such pc found in DB");

			_container.PCSet.Attach(pc).Valid = isPCValid;
		}

		public bool IsPCExists(string hardwareBasedID)
		{
			if (string.IsNullOrEmpty(hardwareBasedID))
				throw new ArgumentException("hardwareBasedID can`t be empty or null");

			return _container.PCSet.Any(el => el.PCHardwareBasedID == hardwareBasedID);
		}

		public PC GetPCByHardwareBasedID(string hardwareBasedID)
		{
			if (string.IsNullOrEmpty(hardwareBasedID))
				throw new ArgumentException("hardwareBasedID can`t be empty or null");

			return _container.PCSet.FirstOrDefault(el => el.PCHardwareBasedID == hardwareBasedID);
		}

		public IEnumerable<PC> GetAllValidPC()
		{
			return _container.PCSet.Where(el => el.Valid).ToArray();
		}  

		public IEnumerable<PC> GetAllNotValidPC()
		{
			return _container.PCSet.Where(el => !el.Valid).ToArray();
		} 
	}
}
