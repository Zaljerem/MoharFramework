using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class HediffCompProperties_PostRemoveTrigger_HediffAdd : HediffCompProperties
{
	public List<HediffDef> triggeredHediff;

	public bool debug = false;

	public HediffCompProperties_PostRemoveTrigger_HediffAdd()
	{
		compClass = typeof(HediffComp_PostRemoveTrigger_HediffAdd);
	}
}
