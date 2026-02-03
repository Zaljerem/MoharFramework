using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace YAHA;

[StaticConstructorOnStartup]
public class EquipPatch
{
	private static class ApplyPatch_PrimaryWeaponChanged
	{
		private static void Postfix_PrimaryWeaponChanged(Pawn ___pawn, ThingWithComps eq)
		{
			if (eq.def.equipmentType == EquipmentType.Primary)
			{
				YahaUtility.UpdateDependingOnTriggerEvent(___pawn, TriggerEvent.weapon);
			}
		}
	}

	public static bool TryPatch_WeaponEquiped(Harmony myPatch)
	{
		try
		{
			MethodBase original = AccessTools.Method(typeof(Pawn_EquipmentTracker), "Notify_EquipmentAdded");
			HarmonyMethod postfix = new HarmonyMethod(typeof(ApplyPatch_PrimaryWeaponChanged), "Postfix_PrimaryWeaponChanged");
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
