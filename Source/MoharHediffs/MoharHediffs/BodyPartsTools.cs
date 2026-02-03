using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MoharHediffs;

public static class BodyPartsTools
{
	public static BodyPartRecord GetBPRWithoutHediff(this Pawn pawn, BodyPartDef bpd, HediffDef hediffDef)
	{
		IEnumerable<BodyPartRecord> source;
		if ((source = from b in pawn.health.hediffSet.GetNotMissingParts()
			where b.def == bpd
			select b) == null)
		{
			return null;
		}
		List<BodyPartRecord> bprToExclude = new List<BodyPartRecord>();
		foreach (Hediff item in pawn.health.hediffSet.hediffs.Where((Hediff h) => h.def == hediffDef))
		{
			if (!bprToExclude.Contains(item.Part))
			{
				bprToExclude.Add(item.Part);
			}
		}
		if (bprToExclude.NullOrEmpty())
		{
			return source.RandomElementWithFallback();
		}
		IEnumerable<BodyPartRecord> source2;
		if ((source2 = source.Where((BodyPartRecord b) => !bprToExclude.Contains(b))) == null)
		{
			return null;
		}
		return source2.RandomElementWithFallback();
	}

	public static bool CheckIfExistingNaturalBP(this Pawn pawn, BodyPartDef bodyPartDef, bool myDebug = false)
	{
		BodyPartRecord bodyPartRecord = pawn.GetBPRecord(bodyPartDef) ?? null;
		if (bodyPartRecord == null || pawn.health.hediffSet.PartIsMissing(bodyPartRecord) || pawn.health.hediffSet.AncestorHasDirectlyAddedParts(bodyPartRecord))
		{
			return false;
		}
		return true;
	}

	public static BodyPartRecord GetBPRecord(this Pawn pawn, BodyPartDef bodyPartDef, bool myDebug = false)
	{
		IEnumerable<BodyPartDef> enumerable = DefDatabase<BodyPartDef>.AllDefs.Where((BodyPartDef b) => b == bodyPartDef);
		if (enumerable.EnumerableNullOrEmpty())
		{
			if (myDebug)
			{
				Log.Warning(pawn.Label + " - GetBPRecord - did not find any " + bodyPartDef?.defName);
			}
			return null;
		}
		BodyPartDef def = enumerable.RandomElement();
		pawn.RaceProps.body.GetPartsWithDef(def).TryRandomElement(out var result);
		if (myDebug)
		{
			Log.Warning(pawn.Label + "GetBPRecord - DID find " + bodyPartDef?.defName);
		}
		return result;
	}

	public static bool IsMissingBPR(this Pawn pawn, BodyPartRecord BPR, out Hediff missingHediff)
	{
		if (BPR == null)
		{
			missingHediff = null;
			return false;
		}
		missingHediff = pawn.health.hediffSet.hediffs.Where((Hediff h) => h.def == HediffDefOf.MissingBodyPart && h.Part == BPR).FirstOrFallback();
		return missingHediff != null;
	}

	public static bool HasMissingChildren(this Pawn pawn, BodyPartRecord bpr)
	{
		List<Hediff_MissingPart> missingPartsCommonAncestors = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
		return missingPartsCommonAncestors.Any((Hediff_MissingPart HMP) => HMP.Part == bpr);
	}

	public static bool IsMissingOrHasMissingChildren(this Pawn pawn, BodyPartRecord bpr)
	{
		return pawn.health.hediffSet.PartIsMissing(bpr) || pawn.HasMissingChildren(bpr);
	}

	public static IEnumerable<BodyPartRecord> GetAllBPR(this Pawn pawn, string bodyPartLabel, BodyPartDef bodyPartDef)
	{
		bool HasLabel = !bodyPartLabel.NullOrEmpty();
		bool HasDef = bodyPartDef != null;
		return pawn.RaceProps.body.AllParts.Where((BodyPartRecord bpr) => (!HasLabel || bpr.customLabel == bodyPartLabel) && (!HasDef || bpr.def == bodyPartDef));
	}

	public static IEnumerable<BodyPartRecord> GetAllNotMissingBPR(this Pawn pawn, string bodyPartLabel, BodyPartDef bodyPartDef)
	{
		bool HasLabel = !bodyPartLabel.NullOrEmpty();
		bool HasDef = bodyPartDef != null;
		return from bpr in pawn.health.hediffSet.GetNotMissingParts()
			where (!HasLabel || bpr.customLabel == bodyPartLabel) && (!HasDef || bpr.def == bodyPartDef)
			select bpr;
	}

	public static BodyPartRecord GetBPRecordWithoutHediff(this Pawn pawn, string bodyPartLabel, BodyPartDef bodyPartDef, HediffDef hd, bool AllowMissing = false, bool PrioritizeMissing = false, bool AllowAddedPart = true, bool myDebug = false)
	{
		bool flag = hd != null;
		bool flag2 = !bodyPartLabel.NullOrEmpty();
		bool flag3 = bodyPartDef != null;
		string text = pawn.Label + " GetBPRecordWithoutHediff - ";
		if (myDebug)
		{
			Log.Warning(text + $"HasDef?{flag3} bodyPartDef:{bodyPartDef?.defName} " + $"HasLabel?{flag2} bodyPartLabel:{bodyPartLabel} " + $"HasHediffDef?{flag} Hediff:{hd?.defName} " + $"AllowMissing:{AllowMissing} PrioritizeMissing:{PrioritizeMissing} AllowAddedPart:{AllowAddedPart}");
		}
		List<BodyPartRecord> bprToExclude = new List<BodyPartRecord>();
		if (flag)
		{
			foreach (Hediff item in pawn.health.hediffSet.hediffs.Where((Hediff h) => h.def == hd))
			{
				if (!bprToExclude.Contains(item.Part))
				{
					bprToExclude.Add(item.Part);
				}
			}
			if (myDebug)
			{
				Log.Warning(text + "found " + bprToExclude?.Count + " bpr to exclude bc they had " + hd.defName);
			}
		}
		BodyPartRecord result = null;
		IEnumerable<BodyPartRecord> enumerable = null;
		if (AllowMissing)
		{
			enumerable = pawn.GetAllBPR(bodyPartLabel, bodyPartDef);
			if (myDebug)
			{
				Log.Warning(text + "Allow missing - found " + (enumerable.EnumerableNullOrEmpty() ? "0" : enumerable.Count().ToString()) + " bpr");
			}
			if (PrioritizeMissing && !enumerable.EnumerableNullOrEmpty() && enumerable.Any((BodyPartRecord bpr) => pawn.IsMissingOrHasMissingChildren(bpr)))
			{
				enumerable = enumerable.Where((BodyPartRecord bpr) => pawn.IsMissingOrHasMissingChildren(bpr));
				if (myDebug)
				{
					Log.Warning(text + "Prioritize Missing - found " + (enumerable.EnumerableNullOrEmpty() ? "0" : enumerable.Count().ToString()) + " bpr");
				}
			}
		}
		else
		{
			enumerable = pawn.GetAllNotMissingBPR(bodyPartLabel, bodyPartDef);
			if (myDebug)
			{
				Log.Warning(text + "Not missing - found " + (enumerable.EnumerableNullOrEmpty() ? "0" : enumerable.Count().ToString()) + " bpr");
			}
		}
		if (enumerable.EnumerableNullOrEmpty())
		{
			return null;
		}
		if (!AllowAddedPart)
		{
			Tools.Warn(text + "Trying to exlude addedpart", myDebug);
			if (enumerable.Any((BodyPartRecord bpr) => pawn.health.hediffSet.HasDirectlyAddedPartFor(bpr)))
			{
				enumerable = enumerable.Where((BodyPartRecord bpr) => !pawn.health.hediffSet.HasDirectlyAddedPartFor(bpr));
				if (myDebug)
				{
					Log.Warning(text + "Added parts(bionics) forbidden- found " + (enumerable.EnumerableNullOrEmpty() ? "0" : enumerable.Count().ToString()) + " bpr");
				}
			}
			else if (myDebug)
			{
				Log.Warning(text + "found no addedpart to exclude");
			}
		}
		if (bprToExclude.NullOrEmpty())
		{
			enumerable.TryRandomElement(out result);
		}
		else if (enumerable.Any((BodyPartRecord bp) => !bprToExclude.Contains(bp)))
		{
			enumerable.Where((BodyPartRecord bp) => !bprToExclude.Contains(bp)).TryRandomElement(out result);
		}
		else
		{
			result = null;
		}
		if (myDebug)
		{
			Log.Warning(pawn.Label + "GetBPRecord - did " + ((result == null) ? "not" : "") + " find with def " + bodyPartDef?.defName + " without hediff def " + hd.defName);
		}
		return result;
	}

	public static BodyPartRecord GetBrain(this Pawn pawn)
	{
		pawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.ConsciousnessSource).TryRandomElement(out var result);
		return result;
	}
}
