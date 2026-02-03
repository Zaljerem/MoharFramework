using System.Collections.Generic;
using Verse;

namespace MoharAiJob;

public class GraveDiggerDef : Def
{
	public List<PawnKindDef> workerPawnKind;

	public List<GraveDig_JobParameters> jobParameters;

	public JobDef jobDef;

	public bool debug = false;

	public bool IsEmpty => jobParameters.NullOrEmpty();

	public override string ToString()
	{
		return defName;
	}

	public GraveDiggerDef Named(string name)
	{
		return DefDatabase<GraveDiggerDef>.GetNamed(name);
	}

	public override int GetHashCode()
	{
		return defName.GetHashCode();
	}
}
