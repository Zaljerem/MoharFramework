using System;
using System.Reflection;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace YAHA;

[StaticConstructorOnStartup]
public class DraftPatch
{
	private static class ApplyPatch_ClearQueuedJobs
	{
		private static void Postfix_ClearQueuedJobs(bool canReturnToPool, Pawn ___pawn)
		{
			if (canReturnToPool)
			{
				YahaUtility.UpdateDependingOnTriggerEvent(___pawn, TriggerEvent.draft);
			}
		}
	}

	public static bool TryPatch_ClearQueuedJobs(Harmony myPatch)
	{
		try
		{
			MethodBase original = AccessTools.Method(typeof(Pawn_JobTracker), "ClearQueuedJobs");
			HarmonyMethod postfix = new HarmonyMethod(typeof(ApplyPatch_ClearQueuedJobs), "Postfix_ClearQueuedJobs");
			myPatch.Patch(original, null, postfix);
		}
		catch (Exception ex)
		{
			Log.Warning("MFW YAHA failed ApplyPatch_ClearQueuedJobs" + ex);
			return false;
		}
		return true;
	}
}
