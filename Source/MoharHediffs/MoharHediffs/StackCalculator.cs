using RimWorld;
using Verse;

namespace MoharHediffs;

public static class StackCalculator
{
	public static float CompletudeRatio(this Pawn pawn, bool myDebug = false)
	{
		float statValue = pawn.GetStatValue(StatDefOf.MeatAmount);
		float statValueFromList = pawn.def.statBases.GetStatValueFromList(StatDefOf.MeatAmount, 75f);
		float num = ((statValueFromList != 0f) ? (statValue / statValueFromList) : pawn.health.summaryHealth.SummaryHealthPercent);
		if (myDebug)
		{
			Log.Warning("pawnWeightedMeat:" + statValue + "; pawnBasisMeat:" + statValueFromList + "=> ratio:" + num);
		}
		return num;
	}

	public static int ComputeSpawnCount(this HediffComp_RandySpawnUponDeath comp)
	{
		float num = comp.NumberToSpawn;
		if (comp.WeightedSpawn)
		{
			num *= comp.Pawn.CompletudeRatio();
		}
		return checked((int)num);
	}
}
