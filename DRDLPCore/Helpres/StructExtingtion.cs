namespace DRDLPCore.Helpres
{
	public static class StructExtingtion
	{
		public static bool IsDefault<T>(this T value) where T : struct
		{
			return value.Equals(default(T));
		}
	}
}
