using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoharAiJob;

public static class RetrieveDefs
{
	public static CorpseJobDef RetrieveCorpseJobDef(this Pawn p, out bool outDebug, bool MyDebug = false)
	{
		string text = (MyDebug ? (p.ThingID + " MoharAiJob.RetrieveDefs.RetrieveCorpseJobDef - ") : string.Empty);
		CorpseJobDef corpseJobDef = DefDatabase<CorpseJobDef>.AllDefs.Where((CorpseJobDef cjd) => cjd.workerPawnKind.Contains(p.kindDef)).FirstOrFallback();
		outDebug = false;
		if (corpseJobDef == null || corpseJobDef.IsEmpty)
		{
			if (MyDebug)
			{
				Log.Warning(string.Concat(text, "found no CorpseJobDef for ", p.kindDef, "; exit"));
			}
			return null;
		}
		outDebug = corpseJobDef.debug;
		return corpseJobDef;
	}

	public static IEnumerable<CorpseRecipeSettings> RetrieveCorpseRecipeSettings(this Pawn p, CorpseJobDef CJD, bool MyDebug = false)
	{
		string text = (MyDebug ? (p.ThingID + " MoharAiJob.RetrieveDefs.RetrieveCorpseRecipeSettings ") : string.Empty);
		IEnumerable<CorpseRecipeSettings> enumerable = p.WorkerFulfillsRequirements(CJD, MyDebug);
		if (enumerable.EnumerableNullOrEmpty())
		{
			if (MyDebug)
			{
				Log.Warning(text + "pawns does not fulfil requirements; exit");
			}
			return null;
		}
		if (!enumerable.Any((CorpseRecipeSettings c) => c.HasTargetSpec))
		{
			if (MyDebug)
			{
				Log.Warning(text + "CRS has no Corpse category def; exit");
			}
			return null;
		}
		return enumerable;
	}
}
