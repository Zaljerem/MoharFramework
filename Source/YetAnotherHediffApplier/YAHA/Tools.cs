using Verse;

namespace YAHA;

public static class Tools
{
	public static bool OkPawn(this Pawn pawn)
	{
		if (pawn != null)
		{
			return pawn.Map != null;
		}
		return false;
	}

	public static string DescriptionAttr<T>(this T source)
	{
		DescriptionAttribute[] array = (DescriptionAttribute[])source.GetType().GetField(source.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
		if (array != null && array.Length != 0)
		{
			return array[0].description;
		}
		return source.ToString();
	}
}
