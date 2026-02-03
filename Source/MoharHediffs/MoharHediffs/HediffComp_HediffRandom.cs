using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoharHediffs;

public class HediffComp_HediffRandom : HediffComp
{
	public HediffCompProperties_HediffRandom Props => (HediffCompProperties_HediffRandom)props;

	private bool myDebug => Props.debug;

	private bool HasWeights => !Props.weights.NullOrEmpty() && Props.weights.Count == Props.hediffPool.Count;

	private bool HasBodyParts => !Props.bodyPartDef.NullOrEmpty() && Props.bodyPartDef.Count == Props.hediffPool.Count;

	private bool HasHediff => !Props.hediffPool.NullOrEmpty();

	private Pawn pawn => parent.pawn;

	public int WeightedRandomness
	{
		get
		{
			int num = 0;
			checked
			{
				foreach (int weight in Props.weights)
				{
					num += weight;
				}
				int num2 = Rand.Range(0, num);
				for (int i = 0; i < Props.weights.Count; i++)
				{
					int num3 = Props.weights[i];
					if ((num2 -= num3) < 0)
					{
						return i;
					}
				}
				return 0;
			}
		}
	}

	public override void CompPostMake()
	{
		if (Props.hideBySeverity)
		{
			parent.Severity = 0.05f;
		}
	}

	public void ApplyHediff(Pawn pawn)
	{
		if (Props.bodyDef != null && pawn.def.race.body != Props.bodyDef)
		{
			Tools.Warn(pawn.Label + " has not a bodyDef like required: " + pawn.def.race.body.ToString() + "!=" + Props.bodyDef.ToString(), myDebug);
			return;
		}
		int num = (HasWeights ? WeightedRandomness : Rand.Range(0, Props.hediffPool.Count()));
		if (num < 0 || num >= Props.hediffPool.Count)
		{
			Tools.Warn(num + " is out of range. Applyhediff will fail. Please report this error.", myDebug);
		}
		HediffDef hediffDef = Props.hediffPool[num];
		if (hediffDef == null)
		{
			Tools.Warn("cant find hediff", myDebug);
			return;
		}
		BodyPartRecord bodyPartRecord = null;
		BodyPartDef bodyPartDef = null;
		if (HasBodyParts)
		{
			bodyPartDef = Props.bodyPartDef[num];
			IEnumerable<BodyPartRecord> partsWithDef = pawn.RaceProps.body.GetPartsWithDef(bodyPartDef);
			if (partsWithDef.EnumerableNullOrEmpty())
			{
				Tools.Warn("cant find body part record called: " + bodyPartDef.defName, myDebug);
				return;
			}
			bodyPartRecord = partsWithDef.RandomElement();
		}
		Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn, bodyPartRecord);
		if (hediff == null)
		{
			Tools.Warn("cant create hediff " + hediffDef.defName + " to apply on " + bodyPartDef?.defName, myDebug);
			return;
		}
		pawn.health.AddHediff(hediff, bodyPartRecord, null);
		Tools.Warn("Succesfully applied " + hediffDef.defName + " to apply on " + bodyPartDef?.defName, myDebug);
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		if (Tools.OkPawn(pawn))
		{
			if (HasHediff)
			{
				ApplyHediff(pawn);
			}
			Tools.DestroyParentHediff(parent, myDebug);
		}
	}
}
