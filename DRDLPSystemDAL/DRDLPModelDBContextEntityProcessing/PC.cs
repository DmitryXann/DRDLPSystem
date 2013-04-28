using System;
using System.Collections.Generic;
using System.Linq;

namespace DRDLPSystemDAL
{
	public partial class DRDLPModelDBContext
	{
		public void AddPC(PC pc)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");

			if (string.IsNullOrEmpty(pc.PCHardwareBasedID))
				throw new ArgumentException("pc.PCHardwareBasedID can`t be empty or null");

			if (pc.Hardware == null)
				throw new ArgumentNullException("pc");

			if (!pc.Hardware.Any())
				throw new ArgumentException("pc.Hardware cant be empty");

			if (!pc.Hardware.All(el => _container.HardwareSet.Contains(el)))
				throw new ArgumentException("all pc.Hardware entities must be in DB");

			_container.PCSet.Add(pc);
		}

		public void AddPC(IEnumerable<Hardware> hardwares, string PCHardwareBasedID, bool valid = true)
		{
			if (hardwares == null)
				throw new ArgumentNullException("hardwares");

			if (!hardwares.Any())
				throw new ArgumentException("pc.Hardware cant be empty");

			if (!hardwares.All(el => _container.HardwareSet.Contains(el)))
				throw new ArgumentException("all pc.Hardware entities must be in DB");

			if (string.IsNullOrEmpty(PCHardwareBasedID))
				throw new ArgumentException("PCHardwareBasedID can`t be empty or null");

			var newPC = new PC {PCHardwareBasedID = PCHardwareBasedID.ToLower().Trim(), Valid = valid};

			foreach (var hardware in hardwares)
			{
				newPC.Hardware.Add(_container.HardwareSet.Attach(hardware));
			}

			_container.PCSet.Add(newPC);
		}

		public void RemovePC(PC pc)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");

			_container.PCSet.Remove(_container.PCSet.Attach(pc));
		}

		public PC AttachPC(PC pc)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");

			return _container.PCSet.Attach(pc);
		}

		public void ChangePCValidation(PC pc,bool isPCValid)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");

			if (!_container.PCSet.Contains(pc))
				throw new ArgumentException("No such pc found in DB");

			_container.PCSet.Attach(pc).Valid = isPCValid;
		}

		public PC GetPCByID(int pcID)
		{
			return _container.PCSet.FirstOrDefault(el => el.Id == pcID);
		}

		public IEnumerable<PC> GetAllValidPC()
		{
			return _container.PCSet.Where(el => el.Valid).ToArray();
		}  

		public IEnumerable<PC> GetAllNotValidPC()
		{
			return _container.PCSet.Where(el => !el.Valid).ToArray();
		} 

		public IEnumerable<PC> GetAllPC()
		{
			return _container.PCSet.ToArray();
		} 

		public long GetPCCount()
		{
			return _container.PCSet.LongCount();
		}
	}
}
