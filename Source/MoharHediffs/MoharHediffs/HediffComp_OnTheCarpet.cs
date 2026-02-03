using Verse;

namespace MoharHediffs;

public class HediffComp_OnTheCarpet : HediffComp
{
	private bool blockAction = false;

	private int graceTime = 999;

	public bool foundFault = false;

	public bool HasItems => Props.HasHediffPool;

	public int ItemCount => Props.ItemCount;

	public bool MyDebug => Props.debug;

	public bool IsTimeToCheck => base.Pawn.IsHashIntervalTick(Props.checkPeriod);

	public HediffCompProperties_OnTheCarpet Props => (HediffCompProperties_OnTheCarpet)props;

	public override string CompTipStringExtra
	{
		get
		{
			string empty = string.Empty;
			return empty + "This should not disappear until an hediff is still there";
		}
	}

	public void BlockAndDestroy(string ErrorLog = "", bool myDebug = false)
	{
		Tools.Warn(ErrorLog, myDebug && !ErrorLog.NullOrEmpty());
		blockAction = true;
		base.Pawn.DestroyHediff(parent, MyDebug);
	}

	public override void CompPostMake()
	{
		base.CompPostMake();
		string text = (MyDebug ? (base.Pawn.LabelShort + " " + parent.def.defName + " CompPostMake - ") : "");
		if (!HasItems)
		{
			Tools.Warn(text + " found no item to work with, destroying ", MyDebug);
			base.Pawn.DestroyHediff(parent, MyDebug);
			blockAction = true;
			return;
		}
		Tools.Warn(text + " found " + ItemCount + " items to work with", MyDebug);
		if (!Props.IsPawnNeedCompatible(base.Pawn))
		{
			Tools.Warn(text + " is not compatible with this hediff, destroying ", MyDebug);
			base.Pawn.DestroyHediff(parent, MyDebug);
			blockAction = true;
		}
		else
		{
			SetGraceTime();
			Tools.Warn(text + " found something to do", MyDebug);
		}
	}

	public void SetGraceTime()
	{
		graceTime = Props.graceTimeBase;
	}

	public override void CompExposeData()
	{
		base.CompExposeData();
		Scribe_Values.Look(ref graceTime, "graceTime", 0);
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		base.CompPostTick(ref severityAdjustment);
		if (!base.Pawn.Negligible() && !blockAction && checked(graceTime--) <= 0)
		{
			bool flag = false;
			if (IsTimeToCheck)
			{
				flag = !this.TreatRelevantHediffsAndReportIfStillHediffsToCheck();
			}
			if (flag)
			{
				base.Pawn.DestroyHediff(parent, MyDebug);
			}
		}
	}
}
