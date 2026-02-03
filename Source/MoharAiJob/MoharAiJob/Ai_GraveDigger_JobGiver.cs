using Verse;
using Verse.AI;

namespace MoharAiJob;

public class Ai_GraveDigger_JobGiver : ThinkNode_JobGiver
{
	public bool MyDebug = false;

	public bool PreRetrieveDebug => Prefs.DevMode && DebugSettings.godMode;

	protected override Job TryGiveJob(Pawn pawn)
	{
		string text = (PreRetrieveDebug ? (pawn.LabelShort + " Ai_GraveDigger_JobGiver TryGiveJob ") : "");
		if (pawn.NegligiblePawn())
		{
			if (PreRetrieveDebug)
			{
				Log.Warning(text + "negligible; exit");
			}
			return null;
		}
		GraveDiggerDef graveDiggerDef = pawn.RetrieveGDD(out MyDebug, PreRetrieveDebug);
		GraveDig_JobParameters graveDig_JobParameters = pawn.RetrieveGDJP(graveDiggerDef, MyDebug);
		if (!pawn.GetClosestCompatibleGrave(graveDig_JobParameters.target, out var grave, out var corpse, MyDebug))
		{
			if (MyDebug)
			{
				Log.Warning(string.Concat(text, "grave or corpse ", grave?.Label, " ", grave?.Position, " is not ok; exit"));
			}
			return null;
		}
		if (MyDebug)
		{
			Log.Warning(string.Concat(text, " accepting ", graveDiggerDef.jobDef.defName, " for grave ", grave?.Label, " ", grave?.Position, " => go go"));
		}
		return JobMaker.MakeJob(graveDiggerDef.jobDef, grave, corpse);
	}
}
