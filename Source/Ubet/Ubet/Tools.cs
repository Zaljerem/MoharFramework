using System.Reflection;
using Verse;

namespace Ubet;

public static class Tools
{
	public static string DescriptionAttr<T>(this T source)
	{
		FieldInfo field = source.GetType().GetField(source.ToString());
		DescriptionAttribute[] array = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
		if (array != null && array.Length != 0)
		{
			return array[0].description;
		}
		return source.ToString();
	}
}
