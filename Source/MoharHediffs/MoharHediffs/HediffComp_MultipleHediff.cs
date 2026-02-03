using Verse;

namespace MoharHediffs;

public class HediffComp_MultipleHediff : HediffComp
{
	private bool blockAction = false;

	private bool MyDebug => Props.debug;

	private string DebugStr => MyDebug ? (base.Pawn.LabelShort + " MultipleHediff " + parent.def.defName + " - ") : "";

	private bool HasSingleBodyRequirement => Props.bodyDef != null;

	private bool HasWhiteList => !Props.bodyDefWhiteList.NullOrEmpty();

	private bool HasBlackList => !Props.bodyDefBlackList.NullOrEmpty();

	private bool WhiteListCompliant => !HasWhiteList || Props.bodyDefWhiteList.Contains(base.Pawn.def.race.body);

	private bool BlackListCompliant => !HasBlackList || !Props.bodyDefBlackList.Contains(base.Pawn.def.race.body);

	private bool HasAccessList => HasWhiteList || HasBlackList;

	public HediffCompProperties_MultipleHediff Props => (HediffCompProperties_MultipleHediff)props;

	public bool HasHediffToApply => !Props.hediffAndBodypart.NullOrEmpty();

	public override string CompTipStringExtra
	{
		get
		{
			string empty = string.Empty;
			return empty + "This should disappear very fast";
		}
	}

	public void CheckProps()
	{
		string text = DebugStr + "CheckProps - ";
		if (!HasHediffToApply)
		{
			Tools.Warn(text + "- empty hediffAndBodypart, destroying", MyDebug);
			base.Pawn.DestroyHediff(parent);
			blockAction = true;
		}
		if (HasSingleBodyRequirement && base.Pawn.def.race.body != Props.bodyDef)
		{
			Tools.Warn(text + " has not a bodyDef like required: " + base.Pawn.def.race.body.ToString() + "!=" + Props.bodyDef.ToString(), MyDebug);
			base.Pawn.DestroyHediff(parent);
			blockAction = true;
		}
		if (HasAccessList)
		{
			bool blackListCompliant = BlackListCompliant;
			bool whiteListCompliant = WhiteListCompliant;
			if (!blackListCompliant || !whiteListCompliant)
			{
				if (MyDebug)
				{
					Log.Warning(text + (HasWhiteList ? $"Props.BodyDefWhiteList contains {Props.bodyDefWhiteList.Count} elements" : "No whitelist") + ", compliant: " + whiteListCompliant + "; " + (HasBlackList ? $"Props.BodyDefBlackList contains {Props.bodyDefBlackList.Count} elements" : "No blacklist") + ", compliant:" + blackListCompliant);
				}
				base.Pawn.DestroyHediff(parent);
				blockAction = true;
			}
			else if (MyDebug)
			{
				Log.Warning(text + " AccessList compliant ok");
			}
		}
		if (Props.hediffAndBodypart.Any((HediffAndBodyPart habp) => habp.bodyPart != null && habp.bodyPartLabel != null))
		{
			Tools.Warn(text + "at least one item has both a bodypart def and a bodypart label, label will be prioritized", MyDebug);
		}
		if (Props.hediffAndBodypart.Any((HediffAndBodyPart habp) => habp.hediff == null))
		{
			Tools.Warn(text + "at least one item has no hediff defined. What will happen ?", MyDebug);
		}
	}

	public void BlockAndDestroy(string ErrorLog = "", bool myDebug = false)
	{
		Tools.Warn(ErrorLog, myDebug && !ErrorLog.NullOrEmpty());
		blockAction = true;
		Tools.DestroyParentHediff(parent, myDebug);
	}

	public override void CompPostMake()
	{
		base.CompPostMake();
		Tools.Warn(DebugStr + "CompPostMake", MyDebug);
		CheckProps();
	}

	public void ApplyHediff(Pawn pawn)
	{
		string text = DebugStr + "ApplyHediff - ";
		for (int i = 0; i < Props.hediffAndBodypart.Count; i = checked(i + 1))
		{
			HediffDef hediff = Props.hediffAndBodypart[i].hediff;
			BodyPartDef bodyPart = Props.hediffAndBodypart[i].bodyPart;
			string bodyPartLabel = Props.hediffAndBodypart[i].bodyPartLabel;
			bool prioritizeMissing = Props.hediffAndBodypart[i].prioritizeMissing;
			bool allowMissing = Props.hediffAndBodypart[i].allowMissing;
			bool regenIfMissing = Props.hediffAndBodypart[i].regenIfMissing;
			bool allowAddedPart = Props.hediffAndBodypart[i].allowAddedPart;
			bool wholeBodyFallback = Props.hediffAndBodypart[i].wholeBodyFallback;
			if (hediff == null)
			{
				Tools.Warn(text + "cant find hediff; i=" + i, debug: true);
				continue;
			}
			BodyPartRecord bodyPartRecord = null;
			if (bodyPartLabel != null || bodyPart != null)
			{
				Tools.Warn(text + "Trying to retrieve BPR with [BP label]:" + bodyPartLabel + " [BP def]:" + bodyPart?.defName, MyDebug);
				bodyPartRecord = pawn.GetBPRecordWithoutHediff(bodyPartLabel, bodyPart, hediff, allowMissing, prioritizeMissing, allowAddedPart, MyDebug);
			}
			if (bodyPartRecord == null)
			{
				Tools.Warn(text + "Could not find a BPR to apply hediff, will pick whole body?" + wholeBodyFallback, MyDebug);
				if (!wholeBodyFallback)
				{
					continue;
				}
			}
			if (allowMissing && regenIfMissing && bodyPartRecord != null && base.Pawn.IsMissingBPR(bodyPartRecord, out var missingHediff))
			{
				Tools.Warn(text + "regenerating " + bodyPartRecord.customLabel, MyDebug);
				base.Pawn.health.RemoveHediff(missingHediff);
			}
			Hediff hediff2 = HediffMaker.MakeHediff(hediff, pawn, bodyPartRecord);
			if (hediff2 == null)
			{
				Tools.Warn(text + "cant create hediff " + hediff.defName + " to apply on " + bodyPart.defName, debug: true);
			}
			else
			{
				pawn.health.AddHediff(hediff2, bodyPartRecord, null);
				Tools.Warn(text + "Applied " + hediff.defName, MyDebug);
			}
		}
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		if (base.Pawn.Negligible())
		{
			return;
		}
		if (blockAction)
		{
			Tools.DestroyParentHediff(parent, MyDebug);
			return;
		}
		if (HasHediffToApply)
		{
			ApplyHediff(base.Pawn);
		}
		base.Pawn.DestroyHediff(parent, MyDebug);
	}
}
