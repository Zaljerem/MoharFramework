using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoharHediffs;

public static class ConditionValidation
{
	public static bool ValidateCompatibilityOfHediffWithPawn(this PawnCondition pCon, Pawn pawn, bool debug = false)
	{
		string text = (debug ? (pawn.LabelShort + " ValidateCompatibilityOfHediffWithPawn - ") : "");
		if (pCon.HasRace && !pCon.race.Contains(pawn.def))
		{
			Tools.Warn(text + " does not belong to the good race", debug);
			return false;
		}
		if (pCon.HasGender && !pCon.gender.Contains(pawn.gender))
		{
			Tools.Warn(text + " does not belong to the good gender", debug);
			return false;
		}
		if (!pCon.ageRange.Includes(pawn.ageTracker.AgeBiologicalYears))
		{
			Tools.Warn(text + " does not have the good age : " + pawn.ageTracker.AgeBiologicalYears + " => " + pCon.ageRange, debug);
			return false;
		}
		Tools.Warn(text + " valid ok", debug);
		return true;
	}

	public static bool InitialHediffConditionCheck(this HediffComp_AnotherRandom comp, bool debug = false)
	{
		string text = (debug ? (comp.Pawn.LabelShort + " InitialHediffConditionCheck - ") : "");
		Tools.Warn(text + " Entering", debug);
		bool result = !comp.GetCompatibleItems().NullOrEmpty();
		Tools.Warn(text + "found anyAppliableItem:" + result, debug);
		return result;
	}

	public static bool GetBPRFromHediffCondition(this BodyPartCondition bpCon, Pawn pawn, out BodyPartRecord BPR, bool debug = false)
	{
		string text = (debug ? (pawn.LabelShort + " GetBPRFromHediffCondition - ") : "");
		BPR = null;
		Tools.Warn(text + " Entering", debug);
		if (bpCon == null)
		{
			Tools.Warn(text + " Found no condition, returning null aka body", debug);
			return true;
		}
		if (!bpCon.HasBPCondition)
		{
			Tools.Warn(text + " Found no BP condition, returning null aka body", debug);
			return true;
		}
		Tools.Warn(text + " Found BP conditions, selecting : Label:" + bpCon.HasLabel + "; Def:" + bpCon.HasDef + "; Tag:" + bpCon.HasTag, debug);
		IEnumerable<BodyPartRecord> enumerable = from bpr in pawn.health.hediffSet.GetNotMissingParts()
			where (!bpCon.HasLabel || bpCon.bodyPartLabel.Any((string s) => s == bpr.customLabel)) && (!bpCon.HasDef || bpCon.bodyPartDef.Any((BodyPartDef d) => d == bpr.def)) && (!bpCon.HasTag || (!bpr.def.tags.NullOrEmpty() && !bpCon.bodyPartTag.Intersect(bpr.def.tags).EnumerableNullOrEmpty()))
			select bpr;
		if (!enumerable.EnumerableNullOrEmpty())
		{
			BPR = enumerable.RandomElement();
			return true;
		}
		Tools.Warn(pawn.LabelShort + " does not have any compatible bodypart", debug);
		return false;
	}
}
