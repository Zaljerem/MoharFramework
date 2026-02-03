using RimWorld;
using Verse;

namespace MoharHediffs;

public class NeedCondition
{
	public NeedDef needDef;

	public FloatRange level;

	public NeedCondition(NeedCondition copyMe)
	{
		needDef = copyMe.needDef;
		level = copyMe.level;
	}

	public NeedCondition()
	{
	}
}
