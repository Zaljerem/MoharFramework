using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Ubet;

public static class StringFloatArgConditionMethods
{
	public static bool PawnHasNeedInRange(this Pawn p, List<string> strParam, List<FloatRange> floatParam)
	{
		if (p.needs == null)
		{
			return false;
		}
		Need foundNeed;
		if ((foundNeed = p.needs.AllNeeds.Where((Need n) => strParam.Contains(n.def.defName)).FirstOrFallback()) != null)
		{
			return floatParam.Any((FloatRange f) => f.Includes(foundNeed.CurLevel));
		}
		return false;
	}
}
