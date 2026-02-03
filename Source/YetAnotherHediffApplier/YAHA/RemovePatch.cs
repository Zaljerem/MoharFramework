using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace YAHA;

[StaticConstructorOnStartup]
public class RemovePatch
{
	private static class ApplyPatch_PrimaryWeaponRemoved
	{
		private static void Postfix_Notify_EquipmentRemoved(Pawn ___pawn, ThingWithComps eq)
		{
			if (eq.def.equipmentType == EquipmentType.Primary)
			{
				YahaUtility.UpdateDependingOnTriggerEvent(___pawn, TriggerEvent.weapon);
			}
		}
	}

	public static bool TryPatch_WeaponRemoved(Harmony myPatch)
	{
		try
		{
			MethodBase original = AccessTools.Method(typeof(Pawn_EquipmentTracker), "Notify_EquipmentRemoved");
			HarmonyMethod postfix = new HarmonyMethod(typeof(ApplyPatch_PrimaryWeaponRemoved), "Postfix_Notify_EquipmentRemoved");
			myPatch.Patch(original, null, postfix);
		}
		catch (Exception ex)
		{
			Log.Warning("MFW YAHA failed ApplyPatch_PrimaryWeaponChanged: " + ex);
			return false;
		}
		return true;
	}
}
