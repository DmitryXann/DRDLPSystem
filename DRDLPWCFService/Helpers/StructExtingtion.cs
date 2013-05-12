namespace DRDLPWCFService.Helpers
{
	internal static class StructExtingtion
	{
		internal static bool IsDefault<T>(this T value) where T : struct
		{
			return value.Equals(default(T));
		}
	}
}
