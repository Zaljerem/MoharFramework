using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace OHFP;

[StaticConstructorOnStartup]
public class CompEggLayer_Patch
{
	private static class AddOHFPToCompEggLayer_Patch
	{
		private static Thing Postfix(Thing __result, Pawn ___fertilizedBy, CompEggLayer __instance)
		{
			Comp_OHFP_Hatcher comp_OHFP_Hatcher;
			if ((comp_OHFP_Hatcher = __result.TryGetComp<Comp_OHFP_Hatcher>()) != null)
			{
				comp_OHFP_Hatcher.hatcheeFaction = __instance.parent.Faction;
				if (__instance.parent is Pawn hatcheeParent)
				{
					comp_OHFP_Hatcher.hatcheeParent = hatcheeParent;
				}
				if (___fertilizedBy != null)
				{
					comp_OHFP_Hatcher.otherParent = ___fertilizedBy;
				}
			}
			return __result;
		}
	}

	public static bool TryCompEggLayerPatch(Harmony myPatch)
	{
		try
		{
			MethodBase original = AccessTools.Method(typeof(CompEggLayer), "ProduceEgg");
			HarmonyMethod prefix = null;
			HarmonyMethod postfix = new HarmonyMethod(typeof(AddOHFPToCompEggLayer_Patch), "Postfix");
			myPatch.Patch(original, prefix, postfix);
		}
		catch (Exception ex)
		{
			Log.Warning("MFW OHFP-Fixed failed TryCompEggLayerPatch" + ex);
			return false;
		}
		return true;
	}
}
