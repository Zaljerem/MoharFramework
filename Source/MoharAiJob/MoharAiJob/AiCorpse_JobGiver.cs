using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MoharAiJob;

public class AiCorpse_JobGiver : ThinkNode_JobGiver
{
	public bool MyDebug = false;

	public bool PreRetrieveDebug => Prefs.DevMode && DebugSettings.godMode;

	protected override Job TryGiveJob(Pawn pawn)
	{
		string text = (PreRetrieveDebug ? (pawn.LabelShort + " AiCorpse_JobGiver TryGiveJob ") : "");
		if (pawn.NegligiblePawn())
		{
			if (PreRetrieveDebug)
			{
				Log.Warning(text + "negligible; exit");
			}
			return null;
		}
		CorpseJobDef corpseJobDef = pawn.RetrieveCorpseJobDef(out MyDebug, PreRetrieveDebug);
		if (corpseJobDef == null)
		{
			if (PreRetrieveDebug)
			{
				Log.Warning(text + " found no CorpseJobDef; exit");
			}
			return null;
		}
		IEnumerable<CorpseRecipeSettings> enumerable = pawn.RetrieveCorpseRecipeSettings(corpseJobDef, MyDebug);
		if (enumerable.EnumerableNullOrEmpty())
		{
			if (MyDebug)
			{
				Log.Warning(text + " found no CorpseRecipeSettings; exit");
			}
			return null;
		}
		foreach (CorpseRecipeSettings item in enumerable)
		{
			Corpse closestCompatibleCorpse = pawn.GetClosestCompatibleCorpse(item.target, MyDebug);
			if (closestCompatibleCorpse.NegligibleThing())
			{
				if (MyDebug)
				{
					Log.Warning(string.Concat(text, "corpse ", closestCompatibleCorpse?.Label, " ", closestCompatibleCorpse?.Position, " is negligible; exit"));
				}
				continue;
			}
			if (MyDebug)
			{
				Log.Warning(string.Concat(text, " accepting ", corpseJobDef.jobDef.defName, " for corpse ", closestCompatibleCorpse?.Label, " ", closestCompatibleCorpse?.Position, " => go go"));
			}
			return JobMaker.MakeJob(corpseJobDef.jobDef, closestCompatibleCorpse);
		}
		return null;
	}
}
