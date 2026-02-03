using Verse;

namespace YAHA;

public class HediffItem
{
	public FloatRange severity = new FloatRange(1f, 1f);

	public HediffDef hediff;

	public bool HasSeverity
	{
		get
		{
			if (severity.min == 1f)
			{
				return severity.max != 1f;
			}
			return true;
		}
	}
}
