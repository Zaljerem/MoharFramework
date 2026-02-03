using System.Linq;
using Verse;

namespace MoharAiJob;

public static class RetrieveGraveDiggerDef
{
	public static GraveDiggerDef RetrieveGDD(this Pawn p, out bool outDebug, bool MyDebug = false)
	{
		string text = (MyDebug ? (p.LabelShort + " RetrieveGraveDiggerDef RetrieveGDD ") : "");
		GraveDiggerDef graveDiggerDef = DefDatabase<GraveDiggerDef>.AllDefs.Where((GraveDiggerDef gdd) => gdd.workerPawnKind.Contains(p.kindDef)).FirstOrFallback();
		outDebug = false;
		if (graveDiggerDef == null || graveDiggerDef.IsEmpty)
		{
			if (MyDebug)
			{
				Log.Warning(string.Concat(text, "found no GraveDiggerDef for ", p.kindDef, "; exit"));
			}
			return null;
		}
		outDebug = graveDiggerDef.debug;
		return graveDiggerDef;
	}

	public static GraveDig_JobParameters RetrieveGDJP(this Pawn p, GraveDiggerDef GDD, bool MyDebug = false)
	{
		string text = (MyDebug ? (p.LabelShort + " RetrieveGraveDiggerDef RetrieveGDJP ") : "");
		GraveDig_JobParameters graveDig_JobParameters = p.WorkerFulfillsRequirements(GDD);
		if (graveDig_JobParameters == null)
		{
			if (MyDebug)
			{
				Log.Warning(text + "pawns does not fulfil requirements; exit");
			}
			return null;
		}
		if (!graveDig_JobParameters.target.HasEligibleGraves)
		{
			if (MyDebug)
			{
				Log.Warning(text + "CRS has no eligible category def; exit");
			}
			return null;
		}
		return graveDig_JobParameters;
	}
}
