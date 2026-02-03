using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace YAHA;

[StaticConstructorOnStartup]
public class HediffRemovedPatch
{
	private static class ApplyPatch_HediffRemoved
	{
		private static void Postfix_PostRemoved(Pawn ___pawn)
		{
			YahaUtility.UpdateDependingOnTriggerEvent(___pawn, TriggerEvent.hediff);
		}
	}

	public static bool TryPatch_HediffRemoved(Harmony myPatch)
	{
		try
		{
			MethodBase original = AccessTools.Method(typeof(Hediff), "PostRemoved");
			HarmonyMethod postfix = new HarmonyMethod(typeof(ApplyPatch_HediffRemoved), "Postfix_PostRemoved");
			myPatch.Patch(original, null, postfix);
		}
		catch (Exception ex)
		{
			Log.Warning("MFW YAHA failed ApplyPatch_HediffRemoved: " + ex);
			return false;
		}
		return true;
	}
}
