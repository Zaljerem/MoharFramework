using RimWorld;
using Verse;

namespace MoharHediffs;

public static class KeepingConditionCompatibility
{
	public static bool IsPawnNeedConditionCompatible(this Pawn p, HediffKeepingCondition HKC, bool debug = false)
	{
		string text = (debug ? (p.Label + " IsPawnNeedConditionCompatible - ") : "");
		if (HKC.HasNeedCondition)
		{
			foreach (NeedCondition need in HKC.needs)
			{
				bool flag = false;
				foreach (Need allNeed in p.needs.AllNeeds)
				{
					Tools.Warn(text + need.needDef.defName + " found in pawn needs, ok", debug);
					flag |= allNeed.def == need.needDef;
				}
				if (!flag)
				{
					Tools.Warn(text + need.needDef.defName + " not found in pawn needs, exiting", debug);
					return false;
				}
			}
		}
		Tools.Warn(text + "is need compatible, ok", debug);
		return true;
	}

	public static bool IsPawnNeedCompatible(this HediffCompProperties_OnTheCarpet Props, Pawn p)
	{
		bool debug = Props.debug;
		string text = (debug ? (p.Label + " IsPawnNeedCompatible - ") : "");
		if (Props.HasDefaultCondition)
		{
			Tools.Warn(text + "checking default condition", debug);
			if (!p.IsPawnNeedConditionCompatible(Props.defaultCondition, debug))
			{
				Tools.Warn(text + "defaultCondition not compatible with pawn, exiting", debug);
				return false;
			}
			Tools.Warn(text + " Compatible with defaultCondition", debug);
		}
		foreach (HediffItemToRemove item in Props.hediffPool)
		{
			if (item.HasSpecificCondition)
			{
				Tools.Warn(text + "checking " + item.hediffDef.defName + " specific condition", debug);
				if (!p.IsPawnNeedConditionCompatible(item.specificCondition, debug))
				{
					Tools.Warn(text + "specificCondition not compatible with pawn, exiting", debug);
					return false;
				}
				Tools.Warn(text + " Compatible with specificCondition", debug);
			}
		}
		return true;
	}
}
