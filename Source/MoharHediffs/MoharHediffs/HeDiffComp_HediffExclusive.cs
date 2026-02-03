using System.Linq;
using Verse;

namespace MoharHediffs;

public class HeDiffComp_HediffExclusive : HediffComp
{
	private const int tickLimiterModulo = 60;

	private bool MyDebug => Props.debug;

	public HeDiffCompProperties_HediffExclusive Props => (HeDiffCompProperties_HediffExclusive)props;

	public bool HasHediffToNullify => !Props.hediffToNullify.NullOrEmpty();

	public bool HasHediffPatternToNullify => !Props.hediffPatternToNullify.NullOrEmpty();

	public bool HasHediffToApply => Props.hediffToApply != null;

	private bool HasWhiteList => !Props.bodyDefWhiteList.NullOrEmpty();

	private bool HasBlackList => !Props.bodyDefBlackList.NullOrEmpty();

	private bool WhiteListCompliant => !HasWhiteList || Props.bodyDefWhiteList.Contains(base.Pawn.def.race.body);

	private bool BlackListCompliant => !HasBlackList || !Props.bodyDefBlackList.Contains(base.Pawn.def.race.body);

	private bool HasAccessList => HasWhiteList || HasBlackList;

	private string DebugStr => MyDebug ? (base.Pawn.LabelShort + " HediffExclusive " + parent.def.defName + " - ") : "";

	public override string CompTipStringExtra
	{
		get
		{
			string empty = string.Empty;
			return empty + "This should disappear very fast";
		}
	}

	private bool PatternMatch(string MyHediffDefname)
	{
		foreach (string item in Props.hediffPatternToNullify)
		{
			if (MyHediffDefname.Contains(item))
			{
				return true;
			}
		}
		return false;
	}

	public void NullifyHediff()
	{
		int num = 0;
		checked
		{
			foreach (Hediff hediff in base.Pawn.health.hediffSet.hediffs)
			{
				Tools.Warn(base.Pawn.Label + " hediff #" + num + ": " + hediff.def.defName, MyDebug);
				int num2 = 0;
				foreach (HediffDef item in Props.hediffToNullify)
				{
					Tools.Warn(" Props.hediffToNullify #" + num2 + ": " + item, MyDebug);
					if (hediff.def == item && Props.hediffToApply != item)
					{
						hediff.Severity = 0f;
						Tools.Warn(hediff.def.defName + " removed", MyDebug);
					}
					num2++;
				}
				num++;
			}
		}
	}

	public void PatternNullifyHediff()
	{
		int num = 0;
		checked
		{
			foreach (Hediff hediff in base.Pawn.health.hediffSet.hediffs)
			{
				if (MyDebug)
				{
					Log.Warning(base.Pawn.LabelShort + " hediff #" + num + ": " + hediff.def.defName);
				}
				int num2 = 0;
				foreach (string item in Props.hediffPatternToNullify)
				{
					if (MyDebug)
					{
						Log.Warning(" Props.hediffPatternToNullify #" + num2 + ": " + item);
					}
					if (PatternMatch(hediff.def.defName))
					{
						hediff.Severity = 0f;
						Tools.Warn(hediff.def.defName + " severity = 0", MyDebug);
					}
					num2++;
				}
				num++;
			}
		}
	}

	public void ApplyHediff()
	{
		HediffDef hediffToApply = Props.hediffToApply;
		if (hediffToApply == null)
		{
			if (MyDebug)
			{
				Log.Warning("cant find hediff called: " + Props.hediffToApply);
			}
			return;
		}
		BodyPartDef bodyPartDef = DefDatabase<BodyPartDef>.AllDefs.Where((BodyPartDef b) => b == Props.bodyPartDef).RandomElementWithFallback();
		BodyPartRecord bodyPartRecord = null;
		if (bodyPartDef != null)
		{
			bodyPartRecord = base.Pawn.RaceProps.body.GetPartsWithDef(bodyPartDef).RandomElementWithFallback();
			if (bodyPartRecord == null)
			{
				if (MyDebug)
				{
					Log.Warning("cant find body part record called: " + Props.bodyPartDef.defName);
				}
				return;
			}
		}
		Hediff hediff = HediffMaker.MakeHediff(hediffToApply, base.Pawn, bodyPartRecord);
		if (hediff == null)
		{
			if (MyDebug)
			{
				Log.Warning("cant create hediff " + hediffToApply.defName + " to apply on " + Props.bodyPartDef.defName);
			}
		}
		else
		{
			base.Pawn.health.AddHediff(hediff, bodyPartRecord, null);
		}
	}

	public bool CheckProps()
	{
		string text = DebugStr + "ApplyHediff - ";
		if (Props.bodyDef != null && base.Pawn.def.race.body != Props.bodyDef)
		{
			if (MyDebug)
			{
				Log.Warning(base.Pawn.Label + " has not a bodyDef like required: " + base.Pawn.def.race.body.ToString() + "!=" + Props.bodyDef.ToString());
			}
			return false;
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
				return false;
			}
			if (MyDebug)
			{
				Log.Warning(text + " AccessList compliant ok");
			}
		}
		return true;
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		if (base.Pawn.Negligible())
		{
			return;
		}
		if (CheckProps())
		{
			if (HasHediffToNullify)
			{
				NullifyHediff();
			}
			if (HasHediffPatternToNullify)
			{
				PatternNullifyHediff();
			}
			if (HasHediffToApply)
			{
				ApplyHediff();
			}
		}
		Tools.DestroyParentHediff(parent, MyDebug);
	}
}
