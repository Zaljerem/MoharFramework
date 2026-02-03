using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MoharHediffs;

public static class FactionPickerUtils
{
	public static float FactionTotalWeight(this List<FactionPickerParameters> FPP)
	{
		float num = 0f;
		for (int i = 0; i < FPP.Count; i = checked(i + 1))
		{
			num += FPP[i].weight;
		}
		return num;
	}

	public static void ComputeRandomFaction(this HediffComp_RandySpawnUponDeath comp)
	{
		if (!comp.ChosenItem.HasFactionParams)
		{
			return;
		}
		int weightedRandomFaction = comp.GetWeightedRandomFaction();
		if (weightedRandomFaction == -1)
		{
			if (comp.MyDebug)
			{
				Log.Warning("ComputeRandomFaction - found no index");
			}
			return;
		}
		FactionPickerParameters factionPickerParameters = comp.ChosenItem.faction[weightedRandomFaction];
		if (comp.MyDebug)
		{
			factionPickerParameters.Dump();
		}
		comp.RandomFaction = comp.GetFaction(factionPickerParameters);
		if (comp.MyDebug)
		{
			Log.Warning("ComputeRandomFaction - found:" + comp.RandomFaction?.GetCallLabel());
		}
	}

	public static int GetWeightedRandomFaction(this HediffComp_RandySpawnUponDeath comp)
	{
		if (!comp.HasChosenPawn || !comp.ChosenItem.HasFactionParams)
		{
			return -1;
		}
		List<FactionPickerParameters> faction = comp.ChosenItem.faction;
		float num = Rand.Range(0f, faction.FactionTotalWeight());
		for (int i = 0; i < faction.Count; i = checked(i + 1))
		{
			if ((num -= faction[i].weight) < 0f)
			{
				if (comp.MyDebug)
				{
					Log.Warning("GetWeightedRandomIndex : returning " + i);
				}
				return i;
			}
		}
		if (comp.MyDebug)
		{
			Log.Warning("GetWeightedRandomFaction : failed to return proper index, returning -1");
		}
		return -1;
	}

	public static Faction GetFaction(this HediffComp_RandySpawnUponDeath comp, FactionPickerParameters FPP)
	{
		FactionDef fDef = comp.GetFactionDef(FPP);
		if (fDef == null)
		{
			return null;
		}
		return Find.FactionManager.AllFactions.Where((Faction F) => F.def == fDef).FirstOrFallback();
	}

	public static FactionDef GetFactionDef(this HediffComp_RandySpawnUponDeath comp, FactionPickerParameters FPP)
	{
		Pawn pawn = comp.Pawn;
		if (FPP.HasInheritedFaction && pawn.Faction != null)
		{
			return pawn.Faction.def;
		}
		if (FPP.HasForcedFaction)
		{
			return FPP.forcedFaction;
		}
		if (FPP.HasPlayerFaction)
		{
			return Faction.OfPlayer.def;
		}
		if (FPP.HasNoFaction)
		{
			return null;
		}
		if (FPP.HasDefaultPawnKindFaction)
		{
			return comp.ChosenItem.pawnKindToSpawn?.defaultFactionDef ?? null;
		}
		return null;
	}
}
