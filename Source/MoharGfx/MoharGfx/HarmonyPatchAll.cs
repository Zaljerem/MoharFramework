using HarmonyLib;
using Verse;

namespace MoharGfx;

[StaticConstructorOnStartup]
internal static class HarmonyPatchAll
{
	static HarmonyPatchAll()
	{
		Harmony harmony = new Harmony("MoharFW.MoharGfx");
		//if (HarmonyPatch_Mote.Try_MoteSpawnSetup_PostfixPatch(harmony))
		//{
		//	Log.Message(harmony.Id + " patched Mote.SpawnSetup successfully.");
		//}
	}
}
