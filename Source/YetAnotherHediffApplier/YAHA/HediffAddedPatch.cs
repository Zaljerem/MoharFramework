using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace YAHA;

[StaticConstructorOnStartup]
public class HediffAddedPatch
{
	private static class ApplyPatch_HediffAdded
	{
		private static void Postfix_PostAdd(Pawn ___pawn)
		{
			YahaUtility.UpdateDependingOnTriggerEvent(___pawn, TriggerEvent.hediff);
		}
	}

	public static bool TryPatch_HediffAdded(Harmony myPatch)
	{
		try
		{
			MethodBase original = AccessTools.Method(typeof(Hediff), "PostAdd");
			HarmonyMethod postfix = new HarmonyMethod(typeof(ApplyPatch_HediffAdded), "Postfix_PostAdd");
			myPatch.Patch(original, null, postfix);
		}
		catch (Exception ex)
		{
			Log.Warning("MFW YAHA failed ApplyPatch_HediffAdded: " + ex);
			return false;
		}
		return true;
	}
}
