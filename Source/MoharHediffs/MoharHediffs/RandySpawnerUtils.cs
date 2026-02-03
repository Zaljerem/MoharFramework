using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MoharHediffs;

public static class RandySpawnerUtils
{
	public static float TotalWeight(this HediffComp_RandySpawner comp)
	{
		float num = 0f;
		List<ItemParameter> itemParameters = comp.Props.itemParameters;
		for (int i = 0; i < itemParameters.Count; i = checked(i + 1))
		{
			num += itemParameters[i].weight;
		}
		return num;
	}

	public static float TotalWeight(this List<RandomFactionParameter> RFP)
	{
		float num = 0f;
		for (int i = 0; i < RFP.Count; i = checked(i + 1))
		{
			num += RFP[i].weight;
		}
		return num;
	}

	public static void ComputeRandomFaction(this HediffComp_RandySpawner comp)
	{
		if (comp.CurIP.HasFactionParams)
		{
			int weightedRandomFaction = comp.GetWeightedRandomFaction();
			if (weightedRandomFaction == -1)
			{
				Tools.Warn("ComputeRandomFaction - found no index", comp.MyDebug);
				return;
			}
			comp.newBorn = comp.CurIP.randomFactionParameters[weightedRandomFaction].newBorn;
			RandomFactionParameter rFP = comp.CurIP.randomFactionParameters[weightedRandomFaction];
			comp.Itemfaction = comp.GetFaction(rFP);
			Tools.Warn("ComputeRandomFaction - found:" + comp.Itemfaction?.GetCallLabel(), comp.MyDebug);
		}
	}

	public static int GetWeightedRandomIndex(this HediffComp_RandySpawner comp)
	{
		float num = Rand.Range(0f, comp.TotalWeight());
		List<ItemParameter> itemParameters = comp.Props.itemParameters;
		for (int i = 0; i < itemParameters.Count; i = checked(i + 1))
		{
			if ((num -= itemParameters[i].weight) < 0f)
			{
				Tools.Warn("GetWeightedRandomIndex : returning " + i, comp.MyDebug);
				return i;
			}
		}
		Tools.Warn("GetWeightedRandomIndex : failed to return proper index, returning -1", comp.MyDebug);
		return -1;
	}

	public static int GetWeightedRandomFaction(this HediffComp_RandySpawner comp)
	{
		if (!comp.HasValidIP || !comp.CurIP.HasFactionParams)
		{
			return -1;
		}
		List<RandomFactionParameter> randomFactionParameters = comp.CurIP.randomFactionParameters;
		float num = Rand.Range(0f, randomFactionParameters.TotalWeight());
		for (int i = 0; i < randomFactionParameters.Count; i = checked(i + 1))
		{
			if ((num -= randomFactionParameters[i].weight) < 0f)
			{
				Tools.Warn("GetWeightedRandomIndex : returning " + i, comp.MyDebug);
				return i;
			}
		}
		Tools.Warn("GetWeightedRandomFaction : failed to return proper index, returning -1", comp.MyDebug);
		return -1;
	}

	public static bool SetRequirementGraceTicks(this HediffComp_RandySpawner comp)
	{
		bool requiresFood = comp.RequiresFood;
		bool requiresHealth = comp.RequiresHealth;
		if (requiresFood || requiresHealth)
		{
			checked
			{
				if (requiresFood)
				{
					comp.hungerReset++;
				}
				else
				{
					comp.healthReset++;
				}
				if (comp.HasValidIP)
				{
					comp.graceTicks = (int)(comp.CurIP.graceDays.RandomInRange * 60000f);
				}
				return true;
			}
		}
		comp.hungerReset = (comp.healthReset = 0);
		return false;
	}

	public static void CheckProps(this HediffComp_RandySpawner comp)
	{
		if (comp.Props.itemParameters.NullOrEmpty())
		{
			comp.BlockAndDestroy(comp.Pawn.Label + " props: no itemParameters - giving up", comp.MyDebug);
		}
		for (int i = 0; i < comp.Props.itemParameters.Count; i = checked(i + 1))
		{
			ItemParameter itemParameter = comp.Props.itemParameters[i];
			if (itemParameter.spawnCount.min > comp.spawnCountErrorLimit || itemParameter.spawnCount.max > comp.spawnCountErrorLimit)
			{
				comp.BlockAndDestroy(comp.Pawn.Label + " props: SpawnCount is too high: >" + comp.spawnCountErrorLimit, comp.MyDebug);
				break;
			}
			if (itemParameter.daysB4Next.min < comp.minDaysB4NextErrorLimit)
			{
				comp.BlockAndDestroy(comp.Pawn.Label + " props: minDaysB4Next is too low: " + itemParameter.daysB4Next.min + "<" + comp.minDaysB4NextErrorLimit, comp.MyDebug);
				break;
			}
			if (!itemParameter.ThingSpawner && !itemParameter.PawnSpawner)
			{
				comp.BlockAndDestroy(comp.Pawn.Label + " props: not a thing nor pawn spawner bc no def for either", comp.MyDebug);
				break;
			}
			if (!itemParameter.HasFactionParams)
			{
				continue;
			}
			foreach (RandomFactionParameter randomFactionParameter in itemParameter.randomFactionParameters)
			{
				if (!randomFactionParameter.IsLegitRandomFactionParameter())
				{
					comp.BlockAndDestroy(comp.Pawn.Label + " faction props: invalid faction params", comp.MyDebug);
					return;
				}
			}
		}
	}

	public static void DumpProps(this HediffComp_RandySpawner comp)
	{
		Tools.Warn("hungerRelative: " + comp.Props.hungerRelative + "; healthRelative: " + comp.Props.healthRelative + "; ", comp.MyDebug);
		for (int i = 0; i < comp.Props.itemParameters.Count; i = checked(i + 1))
		{
			ItemParameter itemParameter = comp.Props.itemParameters[i];
			itemParameter.LogParams(comp.MyDebug);
		}
	}

	public static bool TryFindSpawnCell(this HediffComp_RandySpawner comp, out IntVec3 result)
	{
		Pawn pawn = comp.Pawn;
		ThingDef thingToSpawn = comp.CurIP.thingToSpawn;
		if (pawn.Negligible())
		{
			result = IntVec3.Invalid;
			Tools.Warn("TryFindSpawnCell Null - pawn null", comp.MyDebug);
			return false;
		}
		checked
		{
			foreach (IntVec3 item in GenAdj.CellsAdjacent8Way(pawn).InRandomOrder())
			{
				if (!item.Walkable(pawn.Map))
				{
					continue;
				}
				Building edifice = item.GetEdifice(pawn.Map);
				if ((edifice != null && thingToSpawn.IsEdifice()) || edifice is Building_Door { FreePassage: false } || !GenSight.LineOfSight(pawn.Position, item, pawn.Map, skipFirstCell: false, null, 0, 0))
				{
					continue;
				}
				bool flag = false;
				List<Thing> thingList = item.GetThingList(pawn.Map);
				for (int i = 0; i < thingList.Count; i++)
				{
					Thing thing = thingList[i];
					if (thing.def.category == ThingCategory.Item && (thing.def != thingToSpawn || thing.stackCount > thingToSpawn.stackLimit - comp.calculatedQuantity))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					result = item;
					return true;
				}
			}
			Tools.Warn("TryFindSpawnCell Null - no spawn cell found", comp.MyDebug);
			result = IntVec3.Invalid;
			return false;
		}
	}

	public static Faction GetFaction(this HediffComp_RandySpawner comp, RandomFactionParameter RFP)
	{
		FactionDef fDef = comp.GetFactionDef(RFP);
		if (fDef == null)
		{
			return null;
		}
		return Find.FactionManager.AllFactions.Where((Faction F) => F.def == fDef).FirstOrFallback();
	}

	public static FactionDef GetFactionDef(this HediffComp_RandySpawner comp, RandomFactionParameter RFP)
	{
		Pawn pawn = comp.Pawn;
		if (RFP.HasInheritedFaction)
		{
			return pawn.Faction.def;
		}
		if (RFP.HasForcedFaction)
		{
			return RFP.forcedFaction;
		}
		if (RFP.HasPlayerFaction)
		{
			return Faction.OfPlayerSilentFail.def;
		}
		if (RFP.HasNoFaction)
		{
			return null;
		}
		if (RFP.HasDefaultPawnKindFaction)
		{
			return comp.CurIP.pawnKindToSpawn?.defaultFactionDef ?? null;
		}
		return null;
	}
}
