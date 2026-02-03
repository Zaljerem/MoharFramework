using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MoharHediffs;

public static class RequirementUtils
{
	public static bool FulfilsSeverityRequirement(this HediffComp_RandySpawnUponDeath comp)
	{
		string text = (comp.MyDebug ? (comp.Pawn.LabelShort + " FulfilsSeverityRequirement - ") : "");
		if (comp.MyDebug)
		{
			Log.Warning(text + "Entering");
		}
		if (comp.Pawn == null || !comp.HasHediffRequirement)
		{
			if (comp.MyDebug)
			{
				Log.Warning(text + " null pawn or no requirement");
			}
			return false;
		}
		bool flag = true;
		foreach (HediffRequirementSettings HRS in comp.Props.requirements.hediff)
		{
			if (HRS.hediffDef == null)
			{
				continue;
			}
			IEnumerable<Hediff> enumerable = comp.Pawn.health.hediffSet.hediffs.Where((Hediff h) => h.def == HRS.hediffDef && h.Severity >= HRS.severity.min && h.Severity <= HRS.severity.max);
			bool flag2 = !enumerable.EnumerableNullOrEmpty();
			flag = flag && flag2;
			if (flag2)
			{
				continue;
			}
			if (comp.MyDebug)
			{
				Log.Warning(text + " did not find " + HRS.hediffDef);
			}
			return false;
		}
		return flag;
	}

	public static bool FulfilsThingRequirement(this HediffComp_RandySpawnUponDeath comp, Corpse corpse, out Thing closestThing)
	{
		string text = (comp.MyDebug ? (comp.Pawn.LabelShort + " FulfilsThingRequirement - ") : "");
		if (comp.MyDebug)
		{
			Log.Warning(text + "Entering");
		}
		closestThing = null;
		if (corpse.Negligible() || !comp.HasThingRequirement)
		{
			if (comp.MyDebug)
			{
				Log.Warning(text + " negligeable corpse or no requirement");
			}
			return false;
		}
		bool flag = true;
		foreach (ThingRequirementSettings TRS in comp.Props.requirements.thing)
		{
			if (TRS.thingDef == null)
			{
				continue;
			}
			CompRefuelable fuelComp = null;
			CompPowerTrader powerComp = null;
			IEnumerable<Thing> enumerable = Find.CurrentMap.spawnedThings.Where((Thing t) => t.def == TRS.thingDef && t.Position.DistanceTo(corpse.Position) <= TRS.distance.max && t.Position.DistanceTo(corpse.Position) >= TRS.distance.min && (!TRS.sameFaction || corpse.InnerPawn.Faction == t.Faction) && (!TRS.needsFueled || ((fuelComp = t.TryGetComp<CompRefuelable>()) != null && fuelComp.HasFuel)) && (!TRS.needsPowered || ((powerComp = t.TryGetComp<CompPowerTrader>()) != null && powerComp.PowerOn)));
			bool flag2 = !enumerable.EnumerableNullOrEmpty();
			if (flag2 && (TRS.spawnClose || TRS.spawnInside))
			{
				closestThing = enumerable.MinBy((Thing t) => t.Position.DistanceTo(corpse.Position));
			}
			flag = flag && flag2;
			if (flag2)
			{
				continue;
			}
			if (comp.MyDebug)
			{
				Log.Warning(text + " did not find " + TRS.thingDef);
			}
			return false;
		}
		return flag;
	}

	public static bool FulfilsRequirement(this HediffComp_RandySpawnUponDeath comp, out Thing closestThing)
	{
		closestThing = null;
		if (!comp.HasRequirement)
		{
			return true;
		}
		if (comp.HasHediffRequirement && !comp.FulfilsSeverityRequirement())
		{
			if (comp.MyDebug)
			{
				Log.Warning("hediff requirements not fulfiled");
			}
			return false;
		}
		if (comp.HasThingRequirement && !comp.FulfilsThingRequirement(comp.Pawn.Corpse, out closestThing))
		{
			if (comp.MyDebug)
			{
				Log.Warning("thing requirements not fulfiled");
			}
			return false;
		}
		return true;
	}
}
