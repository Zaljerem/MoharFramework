using Verse;

namespace MoharHediffs;

public class HediffSeverityCondition
{
	public HediffDef hediffDef;

	public FloatRange acceptableSeverity = new FloatRange(0f, 0.5f);

	public bool HasHediffDef => hediffDef != null;

	public HediffSeverityCondition(HediffSeverityCondition copyMe)
	{
		hediffDef = copyMe.hediffDef;
		acceptableSeverity = copyMe.acceptableSeverity;
	}

	public HediffSeverityCondition()
	{
	}
}
