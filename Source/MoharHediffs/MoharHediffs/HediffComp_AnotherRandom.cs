using System.Collections.Generic;
using Verse;

namespace MoharHediffs;

public class HediffComp_AnotherRandom : HediffComp
{
	private bool blockAction = false;

	public bool HasItems => Props.HasHediffPool;

	public int ItemNum => Props.hediffPool.Count;

	public bool MyDebug => Props.debug;

	public bool LowVerbosity => Props.debug && Props.verbosity >= 1;

	public bool MediumVerbosity => Props.debug && Props.verbosity >= 2;

	public bool HighVerbosity => Props.debug && Props.verbosity >= 3;

	public HediffCompProperties_AnotherRandom Props => (HediffCompProperties_AnotherRandom)props;

	public override string CompTipStringExtra
	{
		get
		{
			string empty = string.Empty;
			return empty + "This should disappear very fast";
		}
	}

	public void DumpProps()
	{
		string text = "CheckProps";
		if (!HasItems)
		{
			Tools.Warn(text + "- HediffComp_AnotherRandom; no item found", MyDebug);
			return;
		}
		Tools.Warn(text + "- HediffComp_AnotherRandom; found " + ItemNum + " items", MyDebug);
	}

	public override void CompPostMake()
	{
		base.CompPostMake();
		string text = (MyDebug ? (base.Pawn.LabelShort + " CompPostMake - ") : "");
		if (MyDebug)
		{
			DumpProps();
		}
		if (!HasItems)
		{
			Tools.Warn(text + " found no item to work with, destroying ", MyDebug);
			base.Pawn.DestroyHediff(parent, MyDebug);
			blockAction = true;
			return;
		}
		Tools.Warn(text + " found " + ItemNum + " items to work with", MyDebug);
		if (Props.HasConditionsToApplyHediffs)
		{
			if (!Props.conditionsToApplyHediffs.pawn.ValidateCompatibilityOfHediffWithPawn(base.Pawn, MyDebug) || !Props.conditionsToApplyHediffs.bodyPart.GetBPRFromHediffCondition(base.Pawn, out var _, MyDebug))
			{
				Tools.Warn(text + " is not compatible with this hediff, destroying ", MyDebug);
				base.Pawn.DestroyHediff(parent, MyDebug);
				blockAction = true;
				return;
			}
		}
		else
		{
			Tools.Warn(text + " skipped HasConditionsToApplyHediffs", MyDebug);
		}
		Tools.Warn(text + " checking if at least 1 hediff from " + ItemNum + " is appliable", MyDebug);
		if (!this.InitialHediffConditionCheck(MyDebug))
		{
			Tools.Warn(text + " has found no appliable item, destroying ", MyDebug);
			base.Pawn.DestroyHediff(parent, MyDebug);
			blockAction = true;
		}
		else
		{
			Tools.Warn(text + " found at least 1 appliable hediff", MyDebug);
		}
	}

	public void ApplyHediff(Pawn pawn)
	{
		string text = (MyDebug ? (base.Pawn.LabelShort + " - " + parent.def.defName + " - ApplyHediff") : "");
		List<HediffItem> list = new List<HediffItem>();
		int randomInRange = Props.hediffToApplyNumRange.RandomInRange;
		List<HediffItem> list2 = this.GetCompatibleItems();
		if (list2.NullOrEmpty())
		{
			return;
		}
		Tools.Warn(text + "Trying to apply " + randomInRange + " hediffs among " + list2.Count + " options pool", MyDebug);
		for (int i = 0; i < randomInRange; i = checked(i + 1))
		{
			string text2 = (MyDebug ? ("[" + i + "/" + randomInRange + "]") : "");
			if (!list.NullOrEmpty())
			{
				list2 = list2.GetRemainingItems(list);
				if (list2.NullOrEmpty())
				{
					break;
				}
			}
			Tools.Warn(text + text2 + " " + list2.Count + " options remaining ", MyDebug);
			HediffItem hediffItem = list2.PickRandomWeightedItem();
			if (hediffItem == null)
			{
				Tools.Warn(text + text2 + " null hediffItem, giving up ", MyDebug);
				break;
			}
			Tools.Warn(string.Concat(text, text2, " found a hediffItem:", hediffItem?.hediffDef, ", going on "), MyDebug);
			float randomInRange2 = hediffItem.applyChance.RandomInRange;
			if (!Rand.Chance(randomInRange2))
			{
				Tools.Warn(text + text2 + " rand(" + randomInRange2 + ") == false, nothing is applied", MyDebug);
				if (Props.excludePickedItems && Props.excludeRandomlyNotApplied)
				{
					list.Add(hediffItem);
				}
				continue;
			}
			Tools.Warn(string.Concat(text, text2, " rand(", randomInRange2, ") == true, hediff:", hediffItem?.hediffDef, " will be applied"), MyDebug);
			HediffDef hediffDef = hediffItem.hediffDef;
			if (hediffDef == null)
			{
				Tools.Warn(text + text2 + "cant find hediff, giving up", MyDebug);
				break;
			}
			HediffCondition defaultPlusSpecificHediffCondition = ConditionBuilder.GetDefaultPlusSpecificHediffCondition(Props?.defaultCondition ?? null, hediffItem?.specificCondition ?? null, HighVerbosity);
			if (!defaultPlusSpecificHediffCondition.bodyPart.GetBPRFromHediffCondition(base.Pawn, out var BPR, MyDebug))
			{
				Tools.Warn(text + text2 + " could not find anything suitable, giving up", MyDebug);
				break;
			}
			Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn, BPR);
			if (hediff == null)
			{
				Tools.Warn(text + text2 + "cant create hediff", MyDebug);
				break;
			}
			hediff.Severity = hediffItem.severity.RandomInRange;
			Tools.Warn(text + text2 + " Applying hediff:" + hediffDef.defName + "; bpr:" + ((BPR == null) ? "body" : BPR.def.defName) + "; severity:" + hediff.Severity, MyDebug);
			pawn.health.AddHediff(hediff, BPR, null);
			if (Props.excludePickedItems)
			{
				list.Add(hediffItem);
			}
		}
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		Pawn pawn = parent.pawn;
		if (!Tools.OkPawn(pawn))
		{
			return;
		}
		if (blockAction)
		{
			base.Pawn.DestroyHediff(parent, MyDebug);
			return;
		}
		if (HasItems)
		{
			ApplyHediff(pawn);
		}
		base.Pawn.DestroyHediff(parent, MyDebug);
	}
}
