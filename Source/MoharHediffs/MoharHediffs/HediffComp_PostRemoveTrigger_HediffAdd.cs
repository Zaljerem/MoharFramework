using Verse;

namespace MoharHediffs;

public class HediffComp_PostRemoveTrigger_HediffAdd : HediffComp
{
	private bool blockAction = false;

	public HediffCompProperties_PostRemoveTrigger_HediffAdd Props => (HediffCompProperties_PostRemoveTrigger_HediffAdd)props;

	public bool HasHediffToApply => !Props.triggeredHediff.NullOrEmpty();

	public override string CompTipStringExtra
	{
		get
		{
			string text = string.Empty;
			if (Props.debug)
			{
				text = text + parent.def.defName + " is still alive, aperture science we do what we must";
			}
			return text;
		}
	}

	public void CheckProps()
	{
		if (!HasHediffToApply)
		{
			blockAction = true;
			Tools.DestroyParentHediff(parent, Props.debug);
		}
	}

	public void ApplyHediff(Pawn pawn)
	{
		for (int i = 0; i < Props.triggeredHediff.Count; i = checked(i + 1))
		{
			HediffDef hediffDef = Props.triggeredHediff[i];
			if (hediffDef == null)
			{
				Tools.Warn("cant find hediff; i=" + i, debug: true);
				break;
			}
			Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
			if (hediff == null)
			{
				Tools.Warn("cant create hediff " + hediffDef.defName, debug: true);
				break;
			}
			Tools.Warn("Adding " + hediffDef.defName + "for science", Props.debug);
			pawn.health.AddHediff(hediff, null, null);
		}
	}

	public override void CompPostPostRemoved()
	{
		Pawn pawn = parent.pawn;
		if (!Tools.OkPawn(pawn))
		{
			Tools.DestroyParentHediff(parent, Props.debug);
		}
		else if (!blockAction)
		{
			Tools.Warn(parent.def.defName + " is no more, applying hediff", Props.debug);
			if (HasHediffToApply)
			{
				ApplyHediff(pawn);
			}
		}
	}
}
