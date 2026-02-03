using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace YAHA;

[StaticConstructorOnStartup]
public class UndressPatch
{
	private static class ApplyPatch_ApparelRemoved
	{
		private static void Postfix_Notify_ApparelRemoved(Pawn_ApparelTracker __instance)
		{
			Pawn pawn;
			if ((pawn = __instance.pawn) != null)
			{
				YahaUtility.UpdateDependingOnTriggerEvent(pawn, TriggerEvent.apparel);
			}
		}
	}

	public static bool TryPatch_ApparelUndressed(Harmony myPatch)
	{
		try
		{
			MethodBase original = AccessTools.Method(typeof(Pawn_ApparelTracker), "Notify_ApparelRemoved", new Type[1] { typeof(Apparel) });
			HarmonyMethod postfix = new HarmonyMethod(typeof(ApplyPatch_ApparelRemoved), "Postfix_Notify_ApparelRemoved");
			myPatch.Patch(original, null, postfix);
		}
		catch (Exception ex)
		{
			Log.Warning("MFW YAHA failed TryPatch_ApparelRemoved: " + ex);
			return false;
		}
		return true;
	}
}
