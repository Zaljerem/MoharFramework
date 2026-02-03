using System.Collections.Generic;
using System.Linq;
using Verse;

namespace YAHA;

public static class YahaUtility
{
	public static void CheckTriggeredAssociations(IEnumerable<Hediff> YahaHediffs, TriggerEvent triggerEvent)
	{
		foreach (Hediff item in YahaHediffs.ToList())
		{
			HediffComp_YetAnotherHediffApplier hediffComp_YetAnotherHediffApplier = item.TryGetComp<HediffComp_YetAnotherHediffApplier>();
			bool debug = hediffComp_YetAnotherHediffApplier.Props.debug;
			if (debug)
			{
				Log.Warning("CheckTriggeredAssociations - Found " + item.def.defName + " Yaha hediff");
			}
			IEnumerable<int> triggeredHediffAssociationIndex = hediffComp_YetAnotherHediffApplier.GetTriggeredHediffAssociationIndex(triggerEvent, debug);
			if (triggeredHediffAssociationIndex.EnumerableNullOrEmpty())
			{
				if (debug)
				{
					Log.Warning("No " + item.def.defName + " Yaha hediff found with " + triggerEvent.GetDesc());
				}
				break;
			}
			foreach (int item2 in triggeredHediffAssociationIndex)
			{
				HediffAssociation curHA = hediffComp_YetAnotherHediffApplier.Props.associations[item2];
				AssociatedHediffHistory curAHH = hediffComp_YetAnotherHediffApplier.Registry[item2];
				if (debug)
				{
					Log.Warning("CheckTriggeredAssociations - Found " + triggerEvent.GetDesc() + " ; i=" + item2);
				}
				hediffComp_YetAnotherHediffApplier.CheckSingleHediffAssociation(curHA, curAHH, ContinueIfTriggered: false);
			}
		}
	}

	public static void UpdateDependingOnTriggerEvent(Pawn p, TriggerEvent te, bool debug = false)
	{
		IEnumerable<Hediff> yahaHediffs;
		if ((yahaHediffs = p.health.hediffSet.hediffs.Where((Hediff hi) => hi.TryGetComp<HediffComp_YetAnotherHediffApplier>() != null)) != null)
		{
			CheckTriggeredAssociations(yahaHediffs, te);
		}
	}
}
