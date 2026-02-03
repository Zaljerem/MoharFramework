using System.Linq;
using RimWorld;
using Verse;

namespace MoharHediffs;

public class HediffComp_HediffNullifier : HediffComp
{
	private int LimitedUsageNumber = 0;

	private bool BlockPostTick = false;

	private readonly bool myDebug = false;

	public HediffCompProperties_HediffNullifier Props => (HediffCompProperties_HediffNullifier)props;

	public bool RequiresAtLeastOneBodyPart => !Props.RequiredBodyPart.NullOrEmpty();

	public bool HasMessageToDisplay => Props.showMessage && !Props.nullifyKey.NullOrEmpty();

	public bool DisplayLimitedUsageLeft => HasMessageToDisplay && Props.concatUsageLimit && !Props.limitedKey.NullOrEmpty();

	public bool HasHediffToNullify => !Props.hediffToNullify.NullOrEmpty();

	public bool HasLimitedUsage => Props.limitedUsageNumber != -99;

	public override string CompTipStringExtra
	{
		get
		{
			string empty = string.Empty;
			if (!HasHediffToNullify)
			{
				return empty;
			}
			empty += "Immune to: ";
			foreach (HediffDef item in Props.hediffToNullify)
			{
				empty = empty + item.label + "; ";
			}
			return HasLimitedUsage ? (empty + " " + LimitedUsageNumber + " left") : (empty + " for ever");
		}
	}

	public void BlockAndDestroy()
	{
		Tools.DestroyParentHediff(parent, myDebug);
		BlockPostTick = true;
	}

	public override void CompPostMake()
	{
		if (myDebug)
		{
			Log.Warning(">>>" + parent.def.defName + " - CompPostMake start");
		}
		if (!HasHediffToNullify)
		{
			if (myDebug)
			{
				Log.Warning(parent.def.defName + " has no hediff to nullify, autokill");
			}
			BlockAndDestroy();
		}
		DestroyHediffIfMissingBP();
		if (HasLimitedUsage)
		{
			LimitedUsageNumber = Props.limitedUsageNumber;
		}
	}

	public void DestroyHediffIfMissingBP()
	{
		if (!RequiresAtLeastOneBodyPart)
		{
			return;
		}
		bool flag = false;
		foreach (BodyPartDef item in Props.RequiredBodyPart)
		{
			if (flag = base.Pawn.CheckIfExistingNaturalBP(item))
			{
				break;
			}
		}
		if (!flag)
		{
			if (myDebug)
			{
				Log.Warning(base.Pawn.LabelShort + " does not have any required body part to have an active " + parent.def.defName + ", autokill");
			}
			BlockAndDestroy();
		}
	}

	public override void CompExposeData()
	{
		Scribe_Values.Look(ref LimitedUsageNumber, "LimitedUsageNumber", 0);
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		if (!base.Pawn.IsHashIntervalTick(Props.checkPeriod) || !Tools.OkPawn(base.Pawn))
		{
			return;
		}
		DestroyHediffIfMissingBP();
		if (BlockPostTick)
		{
			return;
		}
		checked
		{
			foreach (Hediff item in base.Pawn.health.hediffSet.hediffs.Where((Hediff h) => Props.hediffToNullify.Contains(h.def)))
			{
				if (myDebug)
				{
					Log.Warning(base.Pawn.Label + " - " + item.def.defName);
				}
				item.Severity = 0f;
				if (myDebug)
				{
					Log.Warning(item.def.defName + " severity = 0");
				}
				if (HasLimitedUsage)
				{
					LimitedUsageNumber--;
					if (LimitedUsageNumber <= 0)
					{
						if (myDebug)
						{
							Log.Warning(parent.def.defName + " has reached its limit usage, autokill");
						}
						Tools.DestroyParentHediff(parent, myDebug);
					}
				}
				if (HasMessageToDisplay)
				{
					string text = Props.nullifyKey.Translate(base.Pawn.LabelShort, item.def.label, base.Pawn.gender.GetPronoun(), base.Pawn.kindDef.race.label);
					if (DisplayLimitedUsageLeft)
					{
						text += Props.limitedKey.Translate(LimitedUsageNumber);
					}
					text += ".";
					Messages.Message(text, MessageTypeDefOf.NeutralEvent);
				}
			}
		}
	}
}
