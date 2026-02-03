using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoharHediffs;

public static class RandomPicker
{
	public static List<HediffItem> GetCompatibleItems(this HediffComp_AnotherRandom comp)
	{
		List<HediffItem> list = new List<HediffItem>();
		foreach (HediffItem item in comp.Props.hediffPool)
		{
			HediffCondition defaultPlusSpecificHediffCondition = ConditionBuilder.GetDefaultPlusSpecificHediffCondition(comp.Props?.defaultCondition ?? null, item?.specificCondition ?? null, comp.HighVerbosity);
			bool num;
			if (!defaultPlusSpecificHediffCondition.HasBodypartCondition)
			{
				if (!defaultPlusSpecificHediffCondition.HasPawnCondition)
				{
					goto IL_009c;
				}
				num = defaultPlusSpecificHediffCondition.pawn.ValidateCompatibilityOfHediffWithPawn(comp.Pawn);
			}
			else
			{
				num = defaultPlusSpecificHediffCondition.bodyPart.GetBPRFromHediffCondition(comp.Pawn, out var _);
			}
			if (!num)
			{
				continue;
			}
			goto IL_009c;
			IL_009c:
			list.Add(item);
		}
		if (!list.NullOrEmpty())
		{
			return list;
		}
		return null;
	}

	public static List<HediffItem> GetRemainingItems(this List<HediffItem> hediffItems, List<HediffItem> AlreadyPickedItems)
	{
		return hediffItems.Where((HediffItem hi) => !AlreadyPickedItems.Contains(hi)).ToList();
	}

	public static float GetWeight(this List<HediffItem> HL)
	{
		float num = 0f;
		foreach (HediffItem item in HL)
		{
			num += item.weight;
		}
		return num;
	}

	public static HediffItem PickRandomWeightedItem(this List<HediffItem> HL, bool debug = false)
	{
		float weight = HL.GetWeight();
		float num = Rand.Range(0f, weight);
		for (int i = 0; i < HL.Count; i = checked(i + 1))
		{
			if ((num -= HL[i].weight) < 0f)
			{
				Tools.Warn("PickRandomWeightedItem : returning " + i, debug);
				return HL[i];
			}
		}
		return null;
	}
}
