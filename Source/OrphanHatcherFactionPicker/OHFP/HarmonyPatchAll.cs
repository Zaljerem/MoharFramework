using HarmonyLib;
using Verse;

namespace OHFP;

[StaticConstructorOnStartup]
internal static class HarmonyPatchAll
{
	static HarmonyPatchAll()
	{
		Harmony myPatch = new Harmony("goudaQuiche.MoharOHFP");
		if (CompEggLayer_Patch.TryCompEggLayerPatch(myPatch))
		{
			//Log.Message("MoharFW OHFP - CompEggLayer_Patch applied");
		}
	}
}
