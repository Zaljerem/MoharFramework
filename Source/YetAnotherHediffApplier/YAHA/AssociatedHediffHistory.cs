using System.Collections.Generic;
using Verse;

namespace YAHA;

public class AssociatedHediffHistory : IExposable
{
	public int appliedNum;

	public bool done;

	public int grace;

	public List<Hediff> appliedHediffs;

	public bool DidSomethingThisTick;

	public bool HasGraceTime => grace > 0;

	public bool HasAppliedHediffs => !appliedHediffs.NullOrEmpty();

	public AssociatedHediffHistory()
	{
		appliedNum = 0;
		done = false;
		grace = 0;
		appliedHediffs = new List<Hediff>();
	}

	public void ExposeData()
	{
		Scribe_Values.Look(ref appliedNum, "appliedNum", 0);
		Scribe_Values.Look(ref done, "done", defaultValue: false);
		Scribe_Values.Look(ref grace, "grace", 0);
		Scribe_Collections.Look(ref appliedHediffs, "appliedHediffs", LookMode.Reference);
		if (Scribe.mode == LoadSaveMode.PostLoadInit && appliedHediffs == null)
		{
			appliedHediffs = new List<Hediff>();
		}
	}
}
