namespace DRDLPSystemDAL.Interfaces
{
	public interface IUser
	{
		string Login { get; set; }
		bool Valid { get; set; }
	}
}
