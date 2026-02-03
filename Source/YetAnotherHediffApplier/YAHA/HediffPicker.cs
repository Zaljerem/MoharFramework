using System.Collections.Generic;
using Verse;

namespace YAHA;

public static class HediffPicker
{
	public static float GetTotalWeight(this List<RandomHediffItem> randomHediffPool)
	{
		float num = 0f;
		foreach (RandomHediffItem item in randomHediffPool)
		{
			num += item.weight;
		}
		return num;
	}

	public static HediffItem PickRandomWeightedItem(this List<RandomHediffItem> HL, bool debug = false)
	{
		float totalWeight = HL.GetTotalWeight();
		float num = Rand.Range(0f, totalWeight);
		for (int i = 0; i < HL.Count; i = checked(i + 1))
		{
			if ((num -= HL[i].weight) < 0f)
			{
				if (debug)
				{
					Log.Warning("PickRandomWeightedItem : returning " + i);
				}
				return HL[i];
			}
		}
		return null;
	}
}
