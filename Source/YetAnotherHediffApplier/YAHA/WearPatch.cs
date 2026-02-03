using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace YAHA;

[StaticConstructorOnStartup]
public class WearPatch
{
	private static class ApplyPatch_ApparelAdded
	{
		private static void Postfix_Notify_ApparelAdded(Pawn_ApparelTracker __instance)
		{
			Pawn pawn;
			if ((pawn = __instance.pawn) != null)
			{
				YahaUtility.UpdateDependingOnTriggerEvent(pawn, TriggerEvent.apparel);
			}
		}
	}

	public static bool TryPatch_ApparelWorn(Harmony myPatch)
	{
		try
		{
			MethodBase original = AccessTools.Method(typeof(Pawn_ApparelTracker), "Notify_ApparelAdded", new Type[1] { typeof(Apparel) });
			HarmonyMethod postfix = new HarmonyMethod(typeof(ApplyPatch_ApparelAdded), "Postfix_Notify_ApparelAdded");
			myPatch.Patch(original, null, postfix);
		}
		catch (Exception ex)
		{
			Log.Warning("MFW YAHA failed TryPatch_ApparelAdded: " + ex);
			return false;
		}
		return true;
	}
}
