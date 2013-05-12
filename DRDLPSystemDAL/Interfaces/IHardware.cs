namespace DRDLPSystemDAL.Interfaces
{
	public interface IHardware
	{
		string HardwareID { get; set; }
		string Name { get; set; }
		string OtherInfo { get; set; }
		HardwareTypeEnum Type { get; set; }
	}
}
