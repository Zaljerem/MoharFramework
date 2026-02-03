using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public static class MentalStatePicker
{
	public static float MSTotalWeight(this List<MentalStateOption> MSO)
	{
		float num = 0f;
		for (int i = 0; i < MSO.Count; i = checked(i + 1))
		{
			num += MSO[i].weight;
		}
		return num;
	}

	public static void ComputeRandomMentalState(this HediffComp_RandySpawnUponDeath comp)
	{
		if (!comp.ChosenItem.HasMentalStateParams)
		{
			return;
		}
		MentalStateDef weightedRandomMentalState = comp.GetWeightedRandomMentalState();
		if (weightedRandomMentalState == null)
		{
			if (comp.MyDebug)
			{
				Log.Warning("ComputeRandomMentalState - found no MentalStateDef");
			}
			return;
		}
		comp.RandomMS = weightedRandomMentalState;
		if (comp.MyDebug)
		{
			Log.Warning("ComputeRandomFaction - found:" + comp.RandomFaction?.GetCallLabel());
		}
	}

	public static MentalStateDef GetWeightedRandomMentalState(this HediffComp_RandySpawnUponDeath comp)
	{
		if (!comp.HasChosenPawn || !comp.ChosenItem.HasMentalStateParams)
		{
			return null;
		}
		List<MentalStateOption> mentalState = comp.ChosenItem.mentalState;
		float num = Rand.Range(0f, mentalState.MSTotalWeight());
		for (int i = 0; i < mentalState.Count; i = checked(i + 1))
		{
			if ((num -= mentalState[i].weight) < 0f)
			{
				if (comp.MyDebug)
				{
					Log.Warning("GetWeightedRandomIndex : returning " + i);
				}
				return mentalState[i].mentalDef;
			}
		}
		if (comp.MyDebug)
		{
			Log.Warning("GetWeightedRandomMentalState : failed to return proper index, returning null");
		}
		return null;
	}
}
