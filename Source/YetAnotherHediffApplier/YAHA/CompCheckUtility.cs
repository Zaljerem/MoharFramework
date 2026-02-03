using System.Collections.Generic;
using Ubet;
using Verse;

namespace YAHA;

public static class CompCheckUtility
{
	public static void CheckAllHediffAssociations(this HediffComp_YetAnotherHediffApplier c)
	{
		for (int i = 0; i < c.Props.associations.Count; i = checked(i + 1))
		{
			HediffAssociation curHA = c.Props.associations[i];
			AssociatedHediffHistory curAHH = c.Registry[i];
			c.CheckSingleHediffAssociation(curHA, curAHH);
		}
	}

	public static void CheckSingleHediffAssociation(this HediffComp_YetAnotherHediffApplier c, HediffAssociation CurHA, AssociatedHediffHistory CurAHH, bool ContinueIfTriggered = true)
	{
		checked
		{
			if (++c.UpdateNumthisTick > c.Props.UpdateNumthisTickLimit && c.MyDebug)
			{
				Log.Warning("Yaha has tried to update " + c.UpdateNumthisTick + " times during this tick. Limit is : " + c.Props.UpdateNumthisTickLimit + ". Is there a recursion problem ?");
			}
			if (CurAHH.DidSomethingThisTick)
			{
				if (c.MyDebug)
				{
					Log.Warning("Yaha registry says something has already been done this tick; exiting");
				}
			}
			else
			{
				if (CurAHH.done || (ContinueIfTriggered && CurHA.specifics.IsTriggered))
				{
					return;
				}
				if (CurAHH.HasGraceTime)
				{
					CurAHH.grace--;
					return;
				}
				if (!NodeCompute.TrunkNodeComputation((Thing)c.Pawn, CurHA.condition.trunk, c.MyDebug))
				{
					c.RemoveHediffAndDeregister(CurHA, CurAHH);
					return;
				}
				List<BodyPartRecord> list = null;
				if (CurHA.HasBodyPartToApplyHediff)
				{
					list = c.Pawn.GetBP(CurHA.bodyPart, c.MyDebug);
					if (list.NullOrEmpty())
					{
						return;
					}
				}
				if (list.NullOrEmpty())
				{
					if (CurHA.HasHediffPool)
					{
						foreach (HediffItem item in CurHA.hediffPool)
						{
							c.ApplyHediffAndRegisterSingleBodyPart(item, CurHA, CurAHH);
						}
					}
					else if (CurHA.HasRandomHediffPool)
					{
						c.ApplyHediffAndRegisterSingleBodyPart(CurHA.randomHediffPool.PickRandomWeightedItem(), CurHA, CurAHH);
					}
				}
				else if (CurHA.HasHediffPool)
				{
					foreach (HediffItem item2 in CurHA.hediffPool)
					{
						c.ApplyHediffAndRegisterWithBodyPartList(item2, CurHA, CurAHH, list);
					}
				}
				else if (CurHA.HasRandomHediffPool)
				{
					c.ApplyHediffAndRegisterWithBodyPartList(CurHA.randomHediffPool.PickRandomWeightedItem(c.MyDebug), CurHA, CurAHH, list);
				}
				if (CurHA.specifics.HasLimit && CurAHH.appliedNum > CurHA.specifics.applyNumLimit)
				{
					CurAHH.done = true;
				}
			}
		}
	}

	public static void ApplyHediffAndRegisterSingleBodyPart(this HediffComp_YetAnotherHediffApplier c, HediffItem hi, HediffAssociation CurHA, AssociatedHediffHistory CurAHH, BodyPartRecord BPR = null)
	{
		Hediff hediff = HediffMaker.MakeHediff(hi.hediff, c.Pawn, BPR);
		if (hi.HasSeverity)
		{
			hediff.Severity = hi.severity.RandomInRange;
		}
		c.Pawn.health.AddHediff(hediff, BPR, null);
		CurAHH.appliedHediffs.Add(hediff);
		checked
		{
			if (CurHA.specifics.HasLimit)
			{
				CurAHH.appliedNum++;
			}
			if (CurHA.specifics.HasDiscard && CurHA.specifics.discard.HasUponApplyDiscard && Rand.Chance(CurHA.specifics.discard.uponApply.chance.RandomInRange))
			{
				c.Pawn.health.RemoveHediff(c.parent);
			}
			if (CurHA.specifics.HasGrace && CurHA.specifics.grace.HasUponApplyGrace && Rand.Chance(CurHA.specifics.grace.uponApply.chance.RandomInRange))
			{
				CurAHH.grace += CurHA.specifics.grace.uponApply.tickAmount.RandomInRange;
			}
			CurAHH.DidSomethingThisTick = true;
		}
	}

	public static void ApplyHediffAndRegisterWithBodyPartList(this HediffComp_YetAnotherHediffApplier c, HediffItem hi, HediffAssociation CurHA, AssociatedHediffHistory CurAHH, List<BodyPartRecord> BPRL)
	{
		foreach (BodyPartRecord item in BPRL)
		{
			c.ApplyHediffAndRegisterSingleBodyPart(hi, CurHA, CurAHH, item);
		}
	}

	public static bool RemoveHediffAndDeregister(this HediffComp_YetAnotherHediffApplier c, HediffAssociation CurHA, AssociatedHediffHistory CurAHH)
	{
		if (c.MyDebug)
		{
			Log.Warning("Entering RemoveHediffAndDeregister");
		}
		if (CurHA.HasSpecifics && !CurHA.specifics.removeIfFalse)
		{
			return false;
		}
		if (CurAHH == null || !CurAHH.HasAppliedHediffs)
		{
			return false;
		}
		checked
		{
			for (int num = CurAHH.appliedHediffs.Count - 1; num >= 0; num--)
			{
				CurAHH.appliedHediffs[num].Severity = 0f;
				if (CurHA.specifics.HasDiscard && CurHA.specifics.discard.HasUponRemoveDiscard && Rand.Chance(CurHA.specifics.discard.uponRemove.chance.RandomInRange))
				{
					c.Pawn.health.RemoveHediff(c.parent);
				}
				if (CurHA.specifics.HasGrace && CurHA.specifics.grace.HasUponRemoveGrace && Rand.Chance(CurHA.specifics.grace.uponRemove.chance.RandomInRange))
				{
					CurAHH.grace += CurHA.specifics.grace.uponRemove.tickAmount.RandomInRange;
				}
				CurAHH.appliedHediffs.RemoveAt(num);
				CurAHH.DidSomethingThisTick = true;
			}
			return true;
		}
	}

	public static IEnumerable<int> GetTriggeredHediffAssociationIndex(this HediffComp_YetAnotherHediffApplier c, TriggerEvent te, bool debug = false)
	{
		for (int i = 0; i < c.Props.associations.Count; i = checked(i + 1))
		{
			if (debug)
			{
				Log.Warning("i:" + i);
			}
			if (c.Props.associations[i].specifics.IsTriggered && c.Props.associations[i].specifics.triggerEvent.Contains(te))
			{
				yield return i;
			}
		}
	}
}
